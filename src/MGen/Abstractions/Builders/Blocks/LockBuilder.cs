using System;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    public static LockBuilder AddLock(this BlockOfCodeBase parent, Code expression) => parent
        .Add(new LockBuilder(parent, expression));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1313-the-lock-statement">
/// Lock Block
/// </see>
/// </summary>
public class LockBuilder : BlockOfCode
{
    internal LockBuilder(BlockOfCodeBase parent, Code expression)
        : base(parent) =>
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).Append("lock (").AppendCode(Expression).AppendLine(")");

    /// <summary>
    /// The expression to get a reference instance to lock.
    /// </summary>
    public Code Expression { get; }
}
