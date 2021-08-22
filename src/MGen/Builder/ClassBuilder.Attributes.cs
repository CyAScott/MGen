using Microsoft.CodeAnalysis;
using System.Linq;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        public IClassBuilder AppendAttributes(ISymbol symbol)
        {
            var attributes = symbol.GetAttributes();
            if (attributes.Length == 0)
            {
                return this;
            }

            var location = symbol.Locations.FirstOrDefault();

            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass == null)
                {
                    continue;
                }

                Append('[');

                String
                    .Append(attribute.AttributeClass.ContainingNamespace)
                    .Append('.')
                    .Append(attribute.AttributeClass.Name);

                if (attribute.ConstructorArguments.Length > 0 ||
                    attribute.NamedArguments.Length > 0)
                {
                    String.Append('(');

                    for (var index = 0; index < attribute.ConstructorArguments.Length; index++)
                    {
                        if (index > 0)
                        {
                            String.Append(", ");
                        }

                        AppendConstant(location, attribute.ConstructorArguments[index]);
                    }

                    if (attribute.NamedArguments.Length > 0)
                    {
                        for (var index = 0; index < attribute.NamedArguments.Length; index++)
                        {
                            if (index > 0 || attribute.ConstructorArguments.Length > 0)
                            {
                                String.Append(", ");
                            }

                            var argument = attribute.NamedArguments[index];

                            String.Append(argument.Key).Append(" = ");

                            AppendConstant(location, argument.Value);
                        }
                    }

                    String.Append(')');
                }


                AppendLine(']');
            }

            return this;
        }

        void AppendConstant(Location? location, TypedConstant constant)
        {
            if (constant.IsNull)
            {
                String.Append("null");
                return;
            }

            if (constant.Kind == TypedConstantKind.Primitive)
            {
                if (constant.Type?.SpecialType == SpecialType.System_String)
                {
                    AppendConstant(constant.Value as string);
                }
                else if (constant.Value is bool boolean)
                {
                    String.Append(boolean ? "true" : "false");
                }
                else
                {
                    String.Append(constant.Value);
                }
                return;
            }

            if (constant.Kind == TypedConstantKind.Type)
            {
                String.Append("typeof(").Append(constant.Value).Append(")");
                return;
            }

            if (constant.Type is IArrayTypeSymbol arrayTypeSymbol)
            {
                AppendConstant(location, constant, arrayTypeSymbol);
                return;
            }

            switch (constant.Type?.TypeKind)
            {
                case TypeKind.Enum:
                    String.Append('(').Append(constant.Type.ContainingNamespace).Append('.').Append(constant.Type.Name).Append(')');

                    if (constant.Value is int integer)
                    {
                        String.Append(integer);
                    }
                    else if (constant.Value is long @long)
                    {
                        String.Append(@long);
                    }
                    else if (constant.Value is byte @byte)
                    {
                        String.Append(@byte);
                    }
                    else
                    {
                        String.Append(constant.Value);
                    }
                    break;
                default:
                    _generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "MG0020",
                            "Unable to copy attribute",
                            "Unable to resolve: {0}",
                            "CompileError",
                            DiagnosticSeverity.Error,
                        true), location, constant));
                    break;
            }
        }

        void AppendConstant(Location? location, TypedConstant constant, IArrayTypeSymbol arrayTypeSymbol)
        {
            var elementType = arrayTypeSymbol.ElementType;

            var values = constant.Values;

            String.Append("new ").Append(elementType.ContainingNamespace).Append('.').Append(elementType.Name);

            if (values.Length == 0)
            {
                String.Append("[0]");
            }
            else
            {
                String.Append("[] { ");

                for (var index = 0; index < values.Length; index++)
                {
                    if (index > 0)
                    {
                        String.Append(", ");
                    }
                    AppendConstant(location, values[index]);
                }

                String.Append(" }");
            }
        }

        void AppendConstant(string? value)
        {
            if (value == null)
            {
                String.Append("null");
                return;
            }

            String.Append('"');

            foreach (var c in value)
            {
                switch (c)
                {
                    case '\a':
                        String.Append(@"\a");
                        break;
                    case '\b':
                        String.Append(@"\b");
                        break;
                    case '\f':
                        String.Append(@"\f");
                        break;
                    case '\n':
                        String.Append(@"\n");
                        break;
                    case '\r':
                        String.Append(@"\r");
                        break;
                    case '\t':
                        String.Append(@"\t");
                        break;
                    case '\v':
                        String.Append(@"\v");
                        break;
                    case '\'':
                        String.Append(@"\'");
                        break;
                    case '\"':
                        String.Append(@"\""");
                        break;
                    case '\\':
                        String.Append(@"\\");
                        break;
                    default:
                        if (c < ' ')
                        {
                            String.Append(@$"\x{(int)c:X2}");
                        }
                        else
                        {
                            String.Append(c);
                        }
                        break;
                }
            }

            String.Append('"');
        }
    }
}
