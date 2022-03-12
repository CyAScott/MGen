using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions;

static partial class StringBuilderExtensions
{
    [DebuggerStepThrough]
    public static StringBuilder AppendConstant(this StringBuilder stringBuilder, TypedConstant constant)
    {
        if (constant.IsNull)
        {
            stringBuilder.Append("null");
            return stringBuilder;
        }

        if (constant.Kind == TypedConstantKind.Primitive)
        {
            if (constant.Type?.SpecialType == SpecialType.System_String)
            {
                stringBuilder.AppendConstant(constant.Value as string);
            }
            else if (constant.Value is bool boolean)
            {
                stringBuilder.Append(boolean ? "true" : "false");
            }
            else
            {
                stringBuilder.Append(constant.Value);
            }
            return stringBuilder;
        }

        if (constant.Kind == TypedConstantKind.Enum)
        {
            stringBuilder.Append('(').AppendType(constant.Type).Append(')').Append(constant.Value);
            return stringBuilder;
        }

        if (constant.Kind == TypedConstantKind.Type)
        {
            stringBuilder.Append("typeof(").Append(constant.Value).Append(')');
            return stringBuilder;
        }

        if (constant.Type is IArrayTypeSymbol arrayTypeSymbol)
        {
            stringBuilder.AppendConstant(constant, arrayTypeSymbol);
            return stringBuilder;
        }

        throw new AppendConstantException();
    }

    [DebuggerStepThrough]
    static void AppendConstant(this StringBuilder stringBuilder, TypedConstant constant, IArrayTypeSymbol arrayTypeSymbol)
    {
        var elementType = arrayTypeSymbol.ElementType;

        var values = constant.Values;

        stringBuilder.Append("new ").AppendType(elementType);

        if (values.Length == 0)
        {
            stringBuilder.Append("[0]");
        }
        else
        {
            stringBuilder.Append("[] { ");

            for (var index = 0; index < values.Length; index++)
            {
                if (index > 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.AppendConstant(values[index]);
            }

            stringBuilder.Append(" }");
        }
    }

    [DebuggerStepThrough]
    public static StringBuilder AppendConstant(this StringBuilder stringBuilder, string? value)
    {
        if (value == null)
        {
            stringBuilder.Append("null");
            return stringBuilder;
        }

        stringBuilder.Append('"');

        foreach (var c in value)
        {
            switch (c)
            {
                case '\0':
                    stringBuilder.Append(@"\0");
                    break;
                case '\a':
                    stringBuilder.Append(@"\a");
                    break;
                case '\b':
                    stringBuilder.Append(@"\b");
                    break;
                case '\f':
                    stringBuilder.Append(@"\f");
                    break;
                case '\n':
                    stringBuilder.Append(@"\n");
                    break;
                case '\r':
                    stringBuilder.Append(@"\r");
                    break;
                case '\t':
                    stringBuilder.Append(@"\t");
                    break;
                case '\v':
                    stringBuilder.Append(@"\v");
                    break;
                case '\'':
                    stringBuilder.Append(@"\'");
                    break;
                case '\"':
                    stringBuilder.Append(@"\""");
                    break;
                case '\\':
                    stringBuilder.Append(@"\\");
                    break;
                default:
                    if (char.IsControl(c))
                    {
                        stringBuilder.Append(@$"\u{(int)c:X4}");
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                    break;
            }
        }

        stringBuilder.Append('"');

        return stringBuilder;
    }
}

public class AppendConstantException : ArgumentException
{
}