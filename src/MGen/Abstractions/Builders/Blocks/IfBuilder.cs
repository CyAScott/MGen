using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static IfBuilder AddIf(this BlockOfCodeBase parent, Code @if) => parent
        .Add(new IfBuilder(parent, @if));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1382-the-if-statement">
/// If, Else If, Else Block
/// </see>
/// </summary>
[DebuggerStepThrough]
public class IfBuilder : BlockOfCode
{
    internal IfBuilder(BlockOfCodeBase parent, Code @if)
        : base(parent)
    {
        Else = new(parent);
        ElseIfs = new(parent);
        If = @if ?? throw new ArgumentNullException(nameof(@if));
    }

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder
            .AppendIndent(IndentLevel).Append("if (").AppendCode(If).AppendLine(")");

    protected override void AppendBody(StringBuilder stringBuilder)
    {
        base.AppendBody(stringBuilder);

        foreach (var elseIf in ElseIfs)
        {
            stringBuilder.AppendCode(elseIf);
        }

        stringBuilder.AppendCode(Else);
    }

    /// <summary>
    /// The if expression for the first if.
    /// </summary>
    public Code If { get; set; }

    public ElseIfCollection ElseIfs { get; }

    public ElseBuilder Else { get; }
}

[DebuggerStepThrough]
public class ElseIfCollection : IReadOnlyCollection<ElseIfBuilder>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _elseIfs.GetEnumerator();

    internal ElseIfCollection(BlockOfCodeBase parent) => _parent = parent;

    public ElseIfBuilder Add(Code @if)
    {
        var item = new ElseIfBuilder(_parent, @if);
        _elseIfs.Add(item);
        return item;
    }

    [ExcludeFromCodeCoverage]
    public ElseIfBuilder this[int index] => _elseIfs[index];

    public IEnumerator<ElseIfBuilder> GetEnumerator() => _elseIfs.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _elseIfs.Count;

    [ExcludeFromCodeCoverage]
    public void Clear() => _elseIfs.Clear();

    [ExcludeFromCodeCoverage]
    public void Remove(ElseIfBuilder item) => _elseIfs.Remove(item);

    [ExcludeFromCodeCoverage]
    public void Remove(int index) => _elseIfs.RemoveAt(index);

    readonly BlockOfCodeBase _parent;

    readonly List<ElseIfBuilder> _elseIfs = new();
}

/// <summary>
/// An else if case for <see cref="IfBuilder"/>.
/// </summary>
[DebuggerStepThrough]
public class ElseIfBuilder : BlockOfCode
{
    internal ElseIfBuilder(BlockOfCodeBase parent, Code @if)
        : base(parent) =>
        If = @if ?? throw new ArgumentNullException(nameof(@if));

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder
            .AppendIndent(IndentLevel).Append("else if (").AppendCode(If).AppendLine(")");

    /// <summary>
    /// The if expression for the else if.
    /// </summary>
    public Code If { get; set; }
}

/// <summary>
/// An else case for <see cref="IfBuilder"/>.
/// </summary>
[DebuggerStepThrough]
public sealed class ElseBuilder : BlockOfCode
{
    internal ElseBuilder(BlockOfCodeBase parent)
        : base(parent) =>
        Enabled = false;

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).AppendLine("else");

    public override void Generate(StringBuilder stringBuilder)
    {
        if (Count > 0)
        {
            base.Generate(stringBuilder);
        }
    }
}
