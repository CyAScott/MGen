using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions;

static partial class StringBuilderExtensions
{
    [DebuggerStepThrough]
    public static StringBuilder AppendType(this StringBuilder stringBuilder, ITypeSymbol? type)
    {
        if (type != null)
        {
            foreach (var symbolDisplayPart in type.ToDisplayParts())
            {
                if (symbolDisplayPart.Kind == SymbolDisplayPartKind.Punctuation)
                {
                    stringBuilder.Append(symbolDisplayPart.ToString());
                }
                else if (symbolDisplayPart.Symbol is ITypeSymbol typeSymbol)
                {
                    switch (typeSymbol.SpecialType)
                    {
                        case SpecialType.System_Object:
                            stringBuilder.Append("object");
                            break;
                        case SpecialType.System_Void:
                            stringBuilder.Append("void");
                            break;
                        case SpecialType.System_Boolean:
                            stringBuilder.Append("bool");
                            break;
                        case SpecialType.System_Char:
                            stringBuilder.Append("char");
                            break;
                        case SpecialType.System_SByte:
                            stringBuilder.Append("sbyte");
                            break;
                        case SpecialType.System_Byte:
                            stringBuilder.Append("byte");
                            break;
                        case SpecialType.System_Int16:
                            stringBuilder.Append("short");
                            break;
                        case SpecialType.System_UInt16:
                            stringBuilder.Append("ushort");
                            break;
                        case SpecialType.System_Int32:
                            stringBuilder.Append("int");
                            break;
                        case SpecialType.System_UInt32:
                            stringBuilder.Append("uint");
                            break;
                        case SpecialType.System_Int64:
                            stringBuilder.Append("long");
                            break;
                        case SpecialType.System_UInt64:
                            stringBuilder.Append("ulong");
                            break;
                        case SpecialType.System_Decimal:
                            stringBuilder.Append("decimal");
                            break;
                        case SpecialType.System_Single:
                            stringBuilder.Append("float");
                            break;
                        case SpecialType.System_Double:
                            stringBuilder.Append("double");
                            break;
                        case SpecialType.System_String:
                            stringBuilder.Append("string");
                            break;
                        default:
                            stringBuilder.Append(symbolDisplayPart.Symbol?.Name);
                            break;
                    }
                }
                else
                {
                    stringBuilder.Append(symbolDisplayPart.Symbol?.Name);
                }
            }
        }

        return stringBuilder;
    }

    [DebuggerStepThrough]
    public static string ToCsString(this ITypeSymbol? type) => type?.ToString()?.Replace("*", "") ?? "";
}