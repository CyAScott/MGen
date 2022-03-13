using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions;

public interface IAmCode
{
    /// <summary>
    /// Generates the code for this item and appends to the <see cref="StringBuilder"/>.
    /// </summary>
    void Generate(StringBuilder stringBuilder);
}

public interface IAmIndentedCode : IAmCode
{
    /// <summary>
    /// The number of indents to append before each write.
    /// </summary>
    int IndentLevel { get; }
}

/// <summary>
/// Represents a line or statement of C# code.
/// </summary>
[DebuggerStepThrough, ExcludeFromCodeCoverage]
public class Code : IAmCode
{
    readonly Action<StringBuilder> _generator;

    public Code(Action<StringBuilder> generator) => _generator = generator;

    public static readonly Code Empty = string.Empty;
    public static readonly Code NewLine = Environment.NewLine;
    public static readonly Code Null = "null";

    public void Generate(StringBuilder stringBuilder) => _generator(stringBuilder);

    public static implicit operator Code(Action<StringBuilder> generator) => new(generator);
    public static implicit operator Code(Func<string> generator) => new(sb => sb.Append(generator));
    public static implicit operator Code(TypedConstant constant) => new(sb => sb.AppendConstant(constant));
    public static implicit operator Code(bool value) => new(sb => sb.Append(value ? "true" : "false"));
    public static implicit operator Code(byte value) => new(sb => sb.Append(value));
    public static implicit operator Code(char value) => new(sb => sb.Append(value));
    public static implicit operator Code(decimal value) => new(sb => sb.Append(value));
    public static implicit operator Code(double value) => new(sb => sb.Append(value));
    public static implicit operator Code(float value) => new(sb => sb.Append(value));
    public static implicit operator Code(int value) => new(sb => sb.Append(value));
    public static implicit operator Code(long value) => new(sb => sb.Append(value));
    public static implicit operator Code(sbyte value) => new(sb => sb.Append(value));
    public static implicit operator Code(short value) => new(sb => sb.Append(value));
    public static implicit operator Code(string value) => new(sb => sb.Append(value));
    public static implicit operator Code(uint value) => new(sb => sb.Append(value));
    public static implicit operator Code(ulong value) => new(sb => sb.Append(value));
    public static implicit operator Code(ushort value) => new(sb => sb.Append(value));
}

public class CodeType : Code
{
    public CodeType(ITypeSymbol type)
        : base(sb => sb.AppendType(type)) =>
        Type = type;

    public ITypeSymbol Type { get; }
}

public static partial class StringBuilderExtensions
{
    [DebuggerStepThrough]
    public static StringBuilder AppendCode(this StringBuilder stringBuilder, IAmCode? code)
    {
        code?.Generate(stringBuilder);
        return stringBuilder;
    }
}