using MGen.Builder.BuilderContext;
using MGen.Collections;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGen.Builder.Writers
{
    partial class WriteConversionSupport
    {
        public ConstructorBuilder ConversionConstructor { get; } = new()
        {
            Modifier = "protected"
        };

        public List<KeyValuePair<string, string>> TryGetValueMethod { get; } = new();

        public bool SupportsConversion { get; set; }

        public static readonly WriteConversionSupport Instance = new();
    }

    partial class WriteConversionSupport : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            ConversionConstructor.Reset();
            ConversionConstructor.Modifier = "protected";
            TryGetValueMethod.Clear();

            SupportsConversion = context.Interface.IsConvertable();

            if (SupportsConversion)
            {
                ConversionConstructor.Add(new ConstructorParameter(context.Interface, "obj", "[System.Diagnostics.CodeAnalysis.NotNullAttribute]ISupportConversion obj"));
                ConversionConstructor.Body.Add(ctx => ctx.Builder
                    .AppendLine("if (obj == null) throw new System.ArgumentNullException(\"obj\");")
                    .AppendLine()
                    .AppendLine("object? value;")
                    .AppendLine());
            }

            next();

            if (!SupportsConversion)
            {
                return;
            }

            context.Builder.AppendConstructor(context, ConversionConstructor);

            context.Builder
                .AppendLine("System.TypeCode System.IConvertible.GetTypeCode() => System.TypeCode.Object;")
                .AppendLine("bool System.IConvertible.ToBoolean(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("byte System.IConvertible.ToByte(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("char System.IConvertible.ToChar(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("double System.IConvertible.ToDouble(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("short System.IConvertible.ToInt16(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("int System.IConvertible.ToInt32(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("long System.IConvertible.ToInt64(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("float System.IConvertible.ToSingle(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("string System.IConvertible.ToString(System.IFormatProvider provider) => ToString();")
                .AppendLine("ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("uint System.IConvertible.ToUInt32(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) => throw new System.NotSupportedException();")
                .AppendLine("object System.IConvertible.ToType(System.Type conversionType, System.IFormatProvider provider)")
                .OpenBrace()
                .AppendLine("foreach (var constructor in conversionType.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))")
                .OpenBrace()
                .AppendLine("var parameters = constructor.GetParameters();")
                .AppendLine("if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(MGen.ISupportConversion)))")
                .OpenBrace()
                .AppendLine("return constructor.Invoke(new object[] { this });")
                .CloseBrace()
                .CloseBrace()
                .AppendLine("throw new System.InvalidCastException();")
                .CloseBrace()
                .AppendLine()
                .AppendLine("bool MGen.ISupportConversion.TryGetValue(string name, out object? value)")
                .OpenBrace()
                .AppendLine("switch (name)")
                .OpenBrace()
                .AppendLine("default:");

            context.Builder.IndentLevel++;
            context.Builder.AppendLine("value = null;").AppendLine("return false;");
            context.Builder.IndentLevel--;

            foreach (var pair in TryGetValueMethod)
            {
                context.Builder
                    .AppendLine(builder => builder.Append("case \"").Append(pair.Key).Append("\":"))
                    .IndentLevel++;
                context.Builder
                    .AppendLine(builder => builder.Append("value = ").Append(pair.Value).Append(";"))
                    .AppendLine("return true;")
                    .IndentLevel--;
            }

            context.Builder.CloseBrace(2);
        }
    }

    partial class WriteConversionSupport : IHandleBuildingMethods
    {
        public void Handle(MethodBuilderContext context, Action next)
        {
            if (context.Explicit ||
                !SupportsConversion ||
                (context.Method.ContainingSymbol.ContainingAssembly.Name == "System.Runtime" &&
                context.Method.ContainingSymbol.ContainingNamespace.Name == "System" &&
                context.Method.ContainingSymbol.Name != "IConvertible") ||
                (context.Method.ContainingSymbol.ContainingAssembly.Name == "MGen.Abstractions" &&
                context.Method.ContainingSymbol.ContainingNamespace.Name == "MGen" &&
                context.Method.ContainingSymbol.Name != "ISupportConversion"))
            {
                next();
            }
        }
    }

    partial class WriteConversionSupport : IHandleBuildingProperties
    {
        public void ConvertRefType(PropertyBuilderContext context) => ConversionConstructor.Body.Add(ctx =>
        {
            var type = context.Primary.Type.ToCsString();

            context.Builder.TryGetNameForImplementation(context.Primary.Type, out var nestedClass);

            ctx.Builder.AppendLine(builder => builder
                .Append("this.").Append(context.FieldName)
                .Append(" = !obj.TryGetValue(\"").Append(context.Primary.Name).Append("\", out value) || value == null ? default : (")
                .Append(type).Append(")System.Convert.ChangeType(value, typeof(").Append(nestedClass ?? type).Append("));"));
        });

        public void ConvertStringType(PropertyBuilderContext context) => ConversionConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder
            .Append("this.").Append(context.FieldName)
            .Append(" = !obj.TryGetValue(\"").Append(context.Primary.Name).Append("\", out value) ? default : value as string ?? value?.ToString();")));

        public void ConvertValueType(PropertyBuilderContext context)
        {
            var type = context.Primary.Type.ToCsString();

            ConversionConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder
                .Append("this.").Append(context.FieldName)
                .Append(" = !obj.TryGetValue(\"").Append(context.Primary.Name).Append("\", out value) ? default : value as ")
                .Append(type).Append("?").Append(" ?? ")
                .Append("(").Append(type).Append(")")
                .Append("System.ComponentModel.TypeDescriptor.GetConverter(")
                .Append("typeof(").Append(type).Append(")).ConvertFrom(value);")));
        }

        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();

            if (!SupportsConversion || context.FieldName == null)
            {
                return;
            }

            TryGetValueMethod.Add(new KeyValuePair<string, string>(context.Primary.Name, context.FieldName));

            if (TryToConvertCollection(context))
            {
                return;
            }

            var type = context.Primary.Type;

            if (type.SpecialType == SpecialType.System_String)
            {
                ConvertStringType(context);
            }
            else if (type.IsValueType)
            {
                ConvertValueType(context);
            }
            else
            {
                ConvertRefType(context);
            }
        }
    }

    partial class WriteConversionSupport
    {
        public Action<CollectionGenerator>? GetElementKey(ITypeSymbol? type, string elementVarName)
        {
            if (type == null)
            {
                return null;
            }

            var keyTypeCsString = type.ToCsString();

            if (type.SpecialType == SpecialType.System_String)
            {
                return it => it.Builder.String
                    .Append(elementVarName).Append(".Key as string ?? ")
                    .Append(elementVarName).Append(".Key?.ToString()");
            }

            if (type.IsValueType)
            {
                return it => it.Builder.String
                    .Append(elementVarName).Append(".Key as ").Append(keyTypeCsString).Append("?")
                    .Append(" ?? ")
                    .Append("(").Append(keyTypeCsString).Append(")")
                    .Append("System.ComponentModel.TypeDescriptor.GetConverter(")
                    .Append("typeof(").Append(keyTypeCsString).Append(")).ConvertFrom(")
                    .Append(elementVarName).Append(")");
            }

            return it => it.Builder.String
                .Append(elementVarName).Append(".Key as ").Append(keyTypeCsString)
                .Append(" ?? ")
                .Append("(").Append(keyTypeCsString).Append(")")
                .Append("System.Convert.ChangeType(")
                .Append(elementVarName).Append(".Key, ")
                .Append("typeof(").Append(keyTypeCsString).Append("))");
        }

        public Action<CollectionGenerator> GetElementValue(ConstructorBuilderContext context, ITypeSymbol type, string elementVarName, int depth = 0)
        {
            if (type.SpecialType == SpecialType.System_String)
            {
                return it => it.Builder.String
                    .Append(elementVarName).Append(".Value as string ?? ")
                    .Append(elementVarName).Append(".Value?.ToString()");
            }

            if (type.IsValueType)
            {
                var valueTypeCsString = type.ToCsString();

                return it => it.Builder.String
                    .Append(elementVarName).Append(".Value as ").Append(valueTypeCsString).Append("?")
                    .Append(" ?? ")
                    .Append("(").Append(valueTypeCsString).Append(")")
                    .Append("System.ComponentModel.TypeDescriptor.GetConverter(")
                    .Append("typeof(").Append(valueTypeCsString).Append(")).ConvertFrom(")
                    .Append(elementVarName).Append(".Value)");
            }

            var arrayType = type as IArrayTypeSymbol;

            if (arrayType != null)
            {
                type = context.GeneratorExecutionContext.Compilation
                    .GetTypeByMetadataName("System.Collections.Generic.List`1")?.Construct(arrayType.ElementType) ?? throw new InvalidOperationException("Unable to resolve System.Collections.Generic.List");
            }

            if (context.CollectionGenerators.TryToGet(context, type, $"collection{depth}", out var nestedCollection))
            {
                if (nestedCollection.HasAdd && nestedCollection.ValueType != null)
                {
                    context.Builder
                        .AppendLine(builder => builder
                            .Append("if (MGen.CollectionHelper.TryToGetEnumerable(").Append(elementVarName).Append(".Value, out var enumerable").Append(depth).Append("))"))
                        .OpenBrace();

                    ConvertCollection(context, nestedCollection, depth);

                    if (arrayType == null)
                    {
                        return it => it.Builder.String.Append(nestedCollection.VariableName);
                    }
                    else
                    {
                        return it => it.Builder.String.Append(nestedCollection.VariableName).Append(".ToArray()");
                    }
                }

                context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "MG0040",
                        "Possible Conversion Issue",
                        "Unable to convert collection type.",
                        "ConversionIssue",
                        DiagnosticSeverity.Warning,
                    true), null));

                return it => it.Builder.String.Append("null");
            }

            {
                var valueTypeCsString = type.ToCsString();

                context.Builder.TryGetNameForImplementation(type, out var nestedClass);

                return it => it.Builder.String
                    .Append("(").Append(valueTypeCsString).Append(")")
                    .Append("System.Convert.ChangeType(")
                    .Append(elementVarName)
                    .Append(".Value, typeof(").Append(nestedClass ?? valueTypeCsString).Append("))");
            }
        }

        public bool TryToConvertCollection(PropertyBuilderContext context)
        {
            var type = context.Primary.Type;

            var arrayType = type as IArrayTypeSymbol;

            if (arrayType != null)
            {
                type = context.GeneratorExecutionContext.Compilation
                    .GetTypeByMetadataName("System.Collections.Generic.List`1")?.Construct(arrayType.ElementType) ?? throw new InvalidOperationException("Unable to resolve System.Collections.Generic.List");
            }

            if (context.CollectionGenerators.TryToGet(context, type, "collection", out var collection))
            {
                if (collection.HasAdd && collection.ValueType != null)
                {
                    ConversionConstructor.Body.Add(ctx =>
                    {
                        ctx.Builder
                            .OpenBrace()
                            .AppendLine(builder => builder
                                .Append("if (obj.TryGetValue(\"").Append(context.Primary.Name).Append("\", out value) && ")
                                .Append("MGen.CollectionHelper.TryToGetEnumerable(value, out var enumerable))"))
                            .OpenBrace();

                        ConvertCollection(ctx, collection);

                        ctx.Builder
                            .AppendLine(builder => builder.Append("this.").Append(context.FieldName)
                                .Append(arrayType == null ? " = collection;" : " = collection.ToArray();"))
                            .CloseBrace()
                            .AppendLine("else")
                            .OpenBrace()
                            .AppendLine(builder => builder.Append("this.").Append(context.FieldName).Append(" = null;"))
                            .CloseBrace(2);
                    });
                }
                else
                {
                    context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "MG0040",
                            "Possible Conversion Issue",
                            "Unable to convert collection type.",
                            "ConversionIssue",
                            DiagnosticSeverity.Warning,
                        true), context.Primary.Locations.FirstOrDefault()));

                    ConversionConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder
                        .Append("this.").Append(context.FieldName).Append(" = null;")));
                }
                return true;
            }

            return false;
        }

        public void ConvertCollection(ConstructorBuilderContext context, CollectionGenerator collection, int depth = 0)
        {
            var elementVarName = depth == 0 ? "element" : $"element{depth}";
            var enumerableVarName = depth == 0 ? "enumerable" : $"enumerable{depth}";

            collection.Create();

            if (collection.Type.Name == "Stack" ||
                collection.Type.Name == "ConcurrentStack")
            {
                context.Builder
                    .AppendLine(it => it.Append("foreach (var ").Append(elementVarName).Append(" in System.Linq.Enumerable.Reverse(").Append(enumerableVarName).Append("))"))
                    .OpenBrace();
            }
            else
            {
                context.Builder
                    .AppendLine(it => it.Append("foreach (var ").Append(elementVarName).Append(" in ").Append(enumerableVarName).Append(")"))
                    .OpenBrace();
            }

            var keyType = collection.KeyType;
            var valueType = collection.ValueType ?? throw new InvalidOperationException();

            var indentLevel = context.Builder.IndentLevel;

            var key = GetElementKey(keyType, elementVarName);
            var value = GetElementValue(context,
                valueType,
                elementVarName,
                depth + 1);

            if (key != null)
            {
                collection.Add(key, value);
            }
            else
            {
                collection.Add(value);
            }

            context.Builder.CloseBrace(context.Builder.IndentLevel - indentLevel + 1);
        }
    }

    static class ConversionExtensions
    {
        public static bool IsConvertable(this ITypeSymbol type)
        {
            var interfaces = type.AllInterfaces;

            for (var index = 0; index < interfaces.Length; index++)
            {
                var @interface = interfaces[index];

                if (@interface.ContainingAssembly.Name == "MGen.Abstractions" &&
                    @interface.ContainingNamespace.Name == "MGen" &&
                    @interface.Name == "ISupportConversion")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
