using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    public static SwitchCaseBuilder AddSwitchCase(this BlockOfCodeBase parent, Code expression) => parent
        .Add(new SwitchCaseBuilder(parent, expression));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1383-the-switch-statement">
/// Switch Case Block
/// </see>
/// </summary>
public class SwitchCaseBuilder : IAmIndentedCode, IHaveEnabled
{
    internal SwitchCaseBuilder(BlockOfCodeBase parent, Code expression)
    {
        Cases = new(this);
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        IndentLevel = parent.IndentLevel + 1;
    }

    /// <summary>
    /// The expression that produces the variable to perform the switch on.
    /// </summary>
    public Code Expression { get; }

    public CaseCollection Cases { get; }

    public bool Enabled { get; set; } = true;

    public int IndentLevel { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        if (Enabled)
        {
            stringBuilder.AppendIndent(IndentLevel).Append("switch (").AppendCode(Expression).AppendLine(")");
            stringBuilder.AppendIndent(IndentLevel).AppendLine("{");
            foreach (var @case in Cases)
            {
                stringBuilder.AppendCode(@case);
            }
            stringBuilder.AppendIndent(IndentLevel).AppendLine("}");
        }
    }
}

public class CaseCollection : IReadOnlyCollection<CaseBuilder>
{
    internal CaseCollection(SwitchCaseBuilder parent) => _parent = parent;

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _cases.GetEnumerator();

    public CaseBuilder Add(Code condition)
    {
        var @case = new CaseBuilder(_parent);
        @case.Conditions.Add(condition);
        _cases.Add(@case);
        return @case;
    }

    public CaseBuilder AddDefault()
    {
        var @case = new CaseBuilder(_parent)
        {
            IsDefault = true
        };
        _cases.Add(@case);
        return @case;
    }

    [ExcludeFromCodeCoverage]
    public CaseBuilder this[int index] => _cases[index];

    [ExcludeFromCodeCoverage]
    public IEnumerator<CaseBuilder> GetEnumerator() => _cases.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _cases.Count;

    [ExcludeFromCodeCoverage]
    public void Clear() => _cases.Clear();

    [ExcludeFromCodeCoverage]
    public void Remove(CaseBuilder item) => _cases.Remove(item);

    [ExcludeFromCodeCoverage]
    public void Remove(int index) => _cases.RemoveAt(index);

    readonly SwitchCaseBuilder _parent;

    readonly List<CaseBuilder> _cases = new();
}

/// <summary>
/// A case for a <see cref="SwitchCaseBuilder"/>.
/// </summary>
public class CaseBuilder : BlockOfCode<SwitchCaseBuilder>
{
    internal CaseBuilder(SwitchCaseBuilder parent)
        : base(parent, parent.IndentLevel + 2)
    {
    }

    internal bool IsDefault { get; set; }

    protected override void AppendHeader(StringBuilder stringBuilder)
    {
        var indentLevel = IndentLevel - 1;

        if (IsDefault)
        {
            stringBuilder.AppendIndent(indentLevel).AppendLine("default:");
        }

        foreach (var condition in Conditions)
        {
            stringBuilder.AppendIndent(indentLevel).Append("case ").AppendCode(condition).AppendLine(":");
        }
    }

    protected override void AppendBody(StringBuilder stringBuilder)
    {
        if (Count > 0)
        {
            base.AppendBody(stringBuilder);
        }
        stringBuilder.AppendIndent(IndentLevel).AppendLine("break;");
    }

    /// <summary>
    /// The condition expressions for this case.
    /// </summary>
    public List<Code> Conditions { get; } = new();
}
