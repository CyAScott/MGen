using System;
using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static DoLoopBuilder AddDoLoop(this BlockOfCodeBase parent, Code condition) => parent
        .Add(new DoLoopBuilder(parent, condition));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1393-the-do-statement">
/// Do Loop Block
/// </see>
/// </summary>
[DebuggerStepThrough]
public class DoLoopBuilder : BlockOfCode
{
    internal DoLoopBuilder(BlockOfCodeBase parent, Code condition)
        : base(parent) =>
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));

    protected override void AppendBody(StringBuilder stringBuilder)
    {
        stringBuilder.AppendIndent(IndentLevel).AppendLine("{");
        foreach (var item in this)
        {
            stringBuilder.AppendCode(item);
        }
        stringBuilder.AppendIndent(IndentLevel).Append("} while (").AppendCode(Condition).AppendLine(");");
    }

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).AppendLine("do");

    /// <summary>
    /// The bool expression evaluated after each iteration and is used to exit the loop when false.
    /// </summary>
    public Code Condition { get; set; }
}
