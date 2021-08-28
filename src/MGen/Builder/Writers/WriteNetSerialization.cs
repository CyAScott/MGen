using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGen.Builder.Writers
{
    partial class WriteNetSerialization
    {
        public WriteNetSerialization()
        {
            DeserializeConstructor.XmlComments.Add("<summary>");
            DeserializeConstructor.XmlComments.Add("Deserializes the class.");
            DeserializeConstructor.XmlComments.Add("</summary>");
            DeserializeConstructor.Modifier = "protected";
            DeserializeConstructor.Body.Add(ctx => ctx.Builder
                .AppendLine("if (info == null) throw new System.ArgumentNullException(\"info\");")
                .AppendLine()
                .AppendLine("foreach (var it in info)")
                .OpenBrace()
                .AppendLine("switch (it.Name)")
                .OpenBrace());
        }

        public ConstructorBuilder DeserializeConstructor { get; } = new();

        public List<Action<ClassBuilderContext>> SerializerBody { get; } = new();

        public bool SupportsSerialization { get; set; }

        public static readonly WriteNetSerialization Instance = new();
    }

    partial class WriteNetSerialization : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            if (DeserializeConstructor.Body.Count > 1)
            {
                DeserializeConstructor.Body.RemoveRange(1, DeserializeConstructor.Body.Count - 1);
            }

            SerializerBody.Clear();
            SupportsSerialization = false;

            foreach (var inheritInterface in context.Interface.AllInterfaces)
            {
                if (inheritInterface.ContainingAssembly.Name == "System.Runtime" &&
                    inheritInterface.ContainingNamespace.Name == "Serialization" &&
                    inheritInterface.Name == "ISerializable")
                {
                    if (DeserializeConstructor.Count == 0)
                    {
                        var serializationMethod = (IMethodSymbol)inheritInterface.GetMembers().Single();

                        DeserializeConstructor.Add(new ConstructorParameter(serializationMethod.Parameters[0].Type, "info", "System.Runtime.Serialization.SerializationInfo info"));
                        DeserializeConstructor.Add(new ConstructorParameter(serializationMethod.Parameters[1].Type, "context", "System.Runtime.Serialization.StreamingContext context"));
                    }

                    SupportsSerialization = true;

                    context.ClassAttributes.Add("System.Serializable");

                    break;
                }
            }

            next();

            if (!SupportsSerialization)
            {
                return;
            }

            DeserializeConstructor.Body.Add(ctx => ctx.Builder.CloseBrace(2));

            context.Builder
                .AppendConstructor(context, DeserializeConstructor)
                .AppendLine("void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)")
                .OpenBrace()
                .AppendLine("if (info == null) throw new System.ArgumentNullException(\"info\");")
                .AppendLine();

            foreach (var action in SerializerBody)
            {
                action(context);
            }

            context.Builder.CloseBrace();
        }
    }

    partial class WriteNetSerialization : IHandleBuildingMethods
    {
        public void Handle(MethodBuilderContext context, Action next)
        {
            if (context.Explicit ||
                !SupportsSerialization ||
                context.Method.Name != "GetObjectData" ||
                context.Method.ContainingSymbol.ContainingAssembly.Name != "System.Runtime" ||
                context.Method.ContainingSymbol.ContainingNamespace.Name != "Serialization" ||
                context.Method.ContainingSymbol.Name != "ISerializable")
            {
                next();
            }
        }
    }

    partial class WriteNetSerialization : IHandleBuildingProperties
    {
        public void WriteSerializationForValue(PropertyBuilderContext context)
        {
            var name = context.Primary.Name;
            var type = context.Primary.Type;
            var typeString = type.ToCsString();

            if (!type.IsSerializable())
            {
                context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "MG0020",
                        "Serialization Issue",
                        "Some values will not be serialized.",
                        "SerializationIssue",
                        DiagnosticSeverity.Warning,
                    true), context.Primary.Locations.FirstOrDefault()));
                return;
            }

            if (type.IsValueType)
            {
                SerializerBody.Add(ctx => ctx.Builder.AppendLine(builder => builder
                    .Append("info.AddValue(\"").Append(name).Append("\", ")
                    .Append(context.FieldName).Append(", ")
                    .Append("typeof(").Append(typeString).Append("));")));
            }
            else
            {
                SerializerBody.Add(ctx => ctx.Builder.AppendLine(builder => builder
                    .Append("info.AddValue(\"").Append(name).Append("\", ")
                    .Append(context.FieldName).Append(", ")
                    .Append(context.FieldName).Append("?.GetType() ?? typeof(").Append(typeString).Append("));")));
            }

            DeserializeConstructor.Body.Add(ctx =>
            {
                ctx.Builder.AppendLine(builder => builder.Append("case \"").Append(name).Append("\":"));
                ctx.Builder.IndentLevel++;
                ctx.Builder.AppendLine(builder => builder.Append("this.").Append(context.FieldName).Append(" = (").Append(typeString).Append(")").Append("it.Value;"));
                ctx.Builder.AppendLine("break;");
                ctx.Builder.IndentLevel--;
            });
        }

        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();

            if (SupportsSerialization && context.FieldName != null)
            {
                WriteSerializationForValue(context);
            }
        }
    }

    static class NetSerializationExtensions
    {
        public static bool IsSerializable(this ITypeSymbol type)
        {
            if (type.IsValueType ||
                type.SpecialType == SpecialType.System_String ||
                type.SpecialType == SpecialType.System_Array)
            {
                return true;
            }

            if (type is IArrayTypeSymbol arrayType)
            {
                return arrayType.ElementType.IsSerializable();
            }

            var interfaces = type.AllInterfaces;

            for (var index = 0; index < interfaces.Length; index++)
            {
                var @interface = interfaces[index];

                if (@interface.ContainingAssembly.Name == "System.Runtime" &&
                    @interface.ContainingNamespace.Name == "Serialization" &&
                    @interface.Name == "ISerializable")
                {
                    return true;
                }
            }

            foreach (var attribute in type.GetAttributes())
            {
                if (attribute.AttributeClass != null &&
                    attribute.AttributeClass.ContainingAssembly.Name == "System.Runtime" &&
                    attribute.AttributeClass.ContainingNamespace.Name == "System" &&
                    attribute.AttributeClass.Name == "SerializableAttribute")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
