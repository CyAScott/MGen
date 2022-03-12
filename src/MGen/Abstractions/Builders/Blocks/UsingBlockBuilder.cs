using System;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    public static UsingBlockBuilder AddUsingBlock(this BlockOfCodeBase parent, Code expression) => parent
        .Add(new UsingBlockBuilder(parent, expression));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1314-the-using-statement">
/// Using Block
/// </see>
/// </summary>
public class UsingBlockBuilder : BlockOfCode
{
    internal UsingBlockBuilder(BlockOfCodeBase parent, Code expression)
        : base(parent) =>
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).Append("using (").AppendCode(Expression).AppendLine(")");

    /// <summary>
    /// The expression to get a reference instance to dispose at the end of the block.
    /// </summary>
    public Code Expression { get; }
}
