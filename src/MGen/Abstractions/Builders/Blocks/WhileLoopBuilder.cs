using System;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    public static WhileLoopBuilder AddWhileLoop(this BlockOfCodeBase parent, Code condition) => parent
        .Add(new WhileLoopBuilder(parent, condition));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1392-the-while-statement">
/// While Loop Block
/// </see>
/// </summary>
public class WhileLoopBuilder : BlockOfCode
{
    internal WhileLoopBuilder(BlockOfCodeBase parent, Code condition)
        : base(parent) =>
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).Append("while (").AppendCode(Condition).AppendLine(")");

    /// <summary>
    /// The bool expression evaluated before each iteration and is used to exit the loop when false.
    /// </summary>
    public Code Condition { get; set; }
}