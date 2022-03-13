using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static ForLoopBuilder AddForLoop(this BlockOfCodeBase parent, Code? initializer = null, Code? condition = null, Code? iterator = null) => parent
        .Add(new ForLoopBuilder(parent)
        {
            Initializer = initializer,
            Condition = condition,
            Iterator = iterator
        });
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1394-the-for-statement">
/// For Loop Block
/// </see>
/// </summary>
[DebuggerStepThrough]
public class ForLoopBuilder : BlockOfCode
{
    internal ForLoopBuilder(BlockOfCodeBase parent)
        : base(parent)
    {
    }

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder
            .AppendIndent(IndentLevel)
            .Append("for (")
            .AppendCode(Initializer).Append("; ")
            .AppendCode(Condition).Append("; ")
            .AppendCode(Iterator).AppendLine(")");

    /// <summary>
    /// The first thing that is execute before the loop starts.
    /// </summary>
    public Code? Initializer { get; set; }

    /// <summary>
    /// The bool expression evaluated before each iteration and is used to exit the loop when false.
    /// </summary>
    public Code? Condition { get; set; }

    /// <summary>
    /// The statement executed after each iteration.
    /// </summary>
    public Code? Iterator { get; set; }
}
