using MGen.Builder.BuilderContext;
using MGen.Collections;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace MGen.Builder.Writers
{
    partial class WriteCloneSupport
    {
        public ConstructorBuilder CloneConstructor { get; } = new();

        public bool SupportsCloneable { get; set; }

        public static readonly WriteCloneSupport Instance = new();
    }

    partial class WriteCloneSupport : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            CloneConstructor.Reset();

            SupportsCloneable = context.Interface.IsCloneable();

            if (SupportsCloneable)
            {
                CloneConstructor.XmlComments.Add("<summary>");
                CloneConstructor.XmlComments.Add("Creates a copy of " + context.ClassName + ".");
                CloneConstructor.XmlComments.Add("</summary>");
                CloneConstructor.Add(new ConstructorParameter(context.Interface, "clone", "[System.Diagnostics.CodeAnalysis.NotNullAttribute]" + context.ClassName + " clone"));
                CloneConstructor.Body.Add(ctx => ctx.Builder.AppendLine("if (clone == null) throw new System.ArgumentNullException(\"clone\");").AppendLine());
            }

            next();

            if (!SupportsCloneable)
            {
                return;
            }

            context.Builder.AppendConstructor(context, CloneConstructor);

            context.Builder.AppendLine("/// <summary>");
            context.Builder.AppendLine("/// Creates a new object that is a copy of the current instance.");
            context.Builder.AppendLine("/// </summary>");
            context.Builder.AppendLine(builder => builder.Append("public virtual object Clone() => new ").Append(context.ClassName).Append("(this);"));
            context.Builder.AppendLine();
        }
    }

    partial class WriteCloneSupport : IHandleBuildingMethods
    {
        public void Handle(MethodBuilderContext context, Action next)
        {
            if (context.Explicit ||
                !SupportsCloneable ||
                context.Method.Name != "Clone" ||
                context.Method.ContainingSymbol.ContainingAssembly.Name != "System.Runtime" ||
                context.Method.ContainingSymbol.ContainingNamespace.Name != "System" ||
                context.Method.ContainingSymbol.Name != "ICloneable")
            {
                next();
            }
        }
    }

    partial class WriteCloneSupport : IHandleBuildingProperties
    {
        public bool TryToCloneCollection(PropertyBuilderContext context)
        {
            var type = context.Primary.Type;

            if (context.CollectionGenerators.TryToGet(context, type, "collection", out var target) &&
                context.CollectionGenerators.TryToGet(context, type, "clone." + context.FieldName, out var source))
            {
                CloneConstructor.Body.Add(_ => CloneCollection(context, target, source));
                return true;
            }

            return false;
        }

        public string CloneObjectStatement(ITypeSymbol type, string value) =>
            "(" + type.ToCsString() + ")(" + value + " is System.ValueType || " + value + " is string ? " + value + " : " +
            "(" + value + " as System.ICloneable)?.Clone() ?? " + value + ")";

        public void CloneValue(PropertyBuilderContext context)
        {
            var type = context.Primary.Type;
            if (type.IsValueType || type.SpecialType == SpecialType.System_String)
            {
                CloneConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder.Append("this.").Append(context.FieldName).Append(" = clone.").Append(context.FieldName).Append(";")));
            }
            else if (type.IsCloneable())
            {
                CloneConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder.Append("this.").Append(context.FieldName).Append(" = (").Append(context.Primary.Type).Append(")clone.").Append(context.FieldName).Append("?.Clone();")));
            }
            else
            {
                context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "MG0010",
                        "Possible Cloning Issue",
                        "Cloning a value without a type can lead to a value being copied instead of cloned.",
                        "CloningIssue",
                        DiagnosticSeverity.Warning,
                    true), context.Primary.Locations.FirstOrDefault()));
                CloneConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder.Append("this.").Append(context.FieldName).Append(" = ").Append(CloneObjectStatement(context.Primary.Type, context.FieldName ?? "")).Append(";")));
            }
        }

        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();

            if (!SupportsCloneable || context.FieldName == null)
            {
                return;
            }

            if (TryToCloneCollection(context))
            {
                return;
            }

            CloneValue(context);
        }

        public void CloneCollection(PropertyBuilderContext context, CollectionGenerator target, CollectionGenerator source, int variablePostFix = 0, Action? set = null)
        {
            var nextVariablePostFix = variablePostFix + 1;

            if (variablePostFix == 0)
            {
                context.Builder
                    .AppendLine(builder => builder.Append("if (").Append(source.VariableName).Append(" == null)"))
                    .OpenBrace()
                    .AppendLine(builder => builder.Append("this.").Append(context.FieldName).Append(" = null;"))
                    .CloseBrace()
                    .AppendLine("else")
                    .OpenBrace();
            }
            else
            {
                context.Builder
                    .AppendLine(builder => builder.Append("if (").Append(source.VariableName).Append(" != null)"))
                    .OpenBrace();
            }

            target.Create(source);

            var type = target.ValueType ?? throw new InvalidCastException("Missing element type.");
            
            if (type.IsValueType || type.SpecialType == SpecialType.System_String)
            {
                source.Enumerate(variablePostFix, (getElementValueExpression, indices) => target.Upsert(indices, getElementValueExpression));
            }
            else if (context.CollectionGenerators.TryToGet(context, type, "collection" + nextVariablePostFix, out var targetElement))
            {
                source.Enumerate(variablePostFix, (getElementValueExpression, indices) =>
                {
                    context.CollectionGenerators.TryToGet(context, type, getElementValueExpression, out var sourceElement);

                    CloneCollection(context, targetElement, sourceElement, nextVariablePostFix, () => target.Upsert(indices, targetElement.VariableName));
                });
            }
            else if (type.IsCloneable())
            {
                source.Enumerate(variablePostFix, (getElementValueExpression, indices) => target.Upsert(indices, "(" + type.ToCsString() + ")((System.ICloneable)" + getElementValueExpression + ")?.Clone()"));
            }
            else
            {
                context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "MG0010",
                        "Possible Cloning Issue",
                        "Cloning a value without a type can lead to a value being copied instead of cloned.",
                        "CloningIssue",
                        DiagnosticSeverity.Warning,
                    true), context.Primary.Locations.FirstOrDefault()));
                source.Enumerate(variablePostFix, (getElementValueExpression, indices) => target.Upsert(indices, CloneObjectStatement(type,  getElementValueExpression)));
            }

            if (variablePostFix == 0)
            {
                context.Builder.AppendLine(builder => builder.Append(variablePostFix == 0 ? "this." : "").Append(context.FieldName).Append(" = ").Append(target.VariableName).Append(';'));
            }
            else
            {
                set?.Invoke();
            }

            context.Builder.CloseBrace();

            if (variablePostFix == 0)
            {
                context.Builder.AppendLine();
            }
        }
    }

    static class CloneExtensions
    {
        public static bool IsCloneable(this ITypeSymbol type)
        {
            var interfaces = type.AllInterfaces;

            foreach (var @interface in interfaces)
            {
                if (@interface.ContainingAssembly.Name == "System.Runtime" &&
                    @interface.ContainingNamespace.Name == "System" &&
                    @interface.Name == "ICloneable")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
