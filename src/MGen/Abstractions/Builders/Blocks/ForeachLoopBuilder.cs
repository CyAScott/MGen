using System;
using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static ForeachLoopBuilder AddForeachLoop(this BlockOfCodeBase parent, string elementType, string elementVariableName, Code enumerable) => parent
        .Add(new ForeachLoopBuilder(parent, elementType, elementVariableName, enumerable));

    [DebuggerStepThrough]
    public static ForeachLoopBuilder AddForeachLoop(this BlockOfCodeBase parent, string elementVariableName, Code enumerable) => parent
        .Add(new ForeachLoopBuilder(parent, "var", elementVariableName, enumerable));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1395-the-foreach-statement">
/// Foreach Loop Block
/// </see>
/// </summary>
[DebuggerStepThrough]
public class ForeachLoopBuilder : BlockOfCode
{
    internal ForeachLoopBuilder(BlockOfCodeBase parent, string elementType, string elementVariableName, Code enumerable)
        : base(parent)
    {
        Enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
        Name = elementVariableName ?? throw new ArgumentNullException(nameof(elementVariableName));
        Type = elementType ?? throw new ArgumentNullException(nameof(elementType));
    }

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder
            .AppendIndent(IndentLevel)
            .Append("foreach (")
            .Append(Type).Append(' ').Append(Name).Append(" in ")
            .AppendCode(Enumerable).AppendLine(")");

    /// <summary>
    /// The expression that produces the variable to enumerate on.
    /// </summary>
    public Code Enumerable { get; set; }

    public string Name { get; }

    public string Type { get; }
}
