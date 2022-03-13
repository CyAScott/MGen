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
    public static TryCatchBuilder AddTryCatch(this BlockOfCodeBase parent, string exceptionType, string variableName, Code? when = null) => parent
        .Add(new TryCatchBuilder(parent, exceptionType, variableName, when));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/statements#1311-the-try-statement">
/// Try Catch Finally Block
/// </see>
/// </summary>
[DebuggerStepThrough]
public class TryCatchBuilder : BlockOfCode
{
    internal TryCatchBuilder(BlockOfCodeBase parent, string exceptionType, string variableName, Code? when = null)
        : base(parent)
    {
        Catches = new CatchCollection(parent);
        Finally = new FinallyBuilder(parent);

        Catches.Add(exceptionType, variableName, when);
    }

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).AppendLine("try");

    protected override void AppendBody(StringBuilder stringBuilder)
    {
        base.AppendBody(stringBuilder);

        foreach (var @catch in Catches)
        {
            stringBuilder.AppendCode(@catch);
        }

        stringBuilder.AppendCode(Finally);
    }

    public CatchCollection Catches { get; }

    public FinallyBuilder Finally { get; }
}

[DebuggerStepThrough]
public class CatchCollection : IReadOnlyCollection<CatchBuilder>
{
    internal CatchCollection(BlockOfCodeBase parent) => _parent = parent;

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _catches.GetEnumerator();

    readonly BlockOfCodeBase _parent;
    readonly List<CatchBuilder> _catches = new();

    public CatchBuilder Add(string exceptionType, string variableName, Code? when)
    {
        var item = new CatchBuilder(_parent, exceptionType, variableName)
        {
            When = when
        };
        _catches.Add(item);
        return item;
    }

    [ExcludeFromCodeCoverage]
    public CatchBuilder this[int index] => _catches[index];

    [ExcludeFromCodeCoverage]
    public IEnumerator<CatchBuilder> GetEnumerator() => _catches.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _catches.Count;

    [ExcludeFromCodeCoverage]
    public void Clear() => _catches.Clear();

    [ExcludeFromCodeCoverage]
    public void Remove(CatchBuilder item) => _catches.Remove(item);

    [ExcludeFromCodeCoverage]
    public void Remove(int index) => _catches.RemoveAt(index);
}

[DebuggerStepThrough]
public class CatchBuilder : BlockOfCode
{
    internal CatchBuilder(BlockOfCodeBase parent, string type, string name)
        : base(parent)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    protected override void AppendHeader(StringBuilder stringBuilder)
    {
        stringBuilder.AppendIndent(IndentLevel).Append("catch (").Append(Type).Append(' ').Append(Name).Append(')');

        if (When == null)
        {
            stringBuilder.AppendLine();
        }
        else
        {
            stringBuilder.Append(" when (").AppendCode(When).AppendLine(")");
        }
    }

    /// <summary>
    /// The optional when boolean expression.
    /// </summary>
    public Code? When { get; set; }

    public string Name { get; }

    public string Type { get; }
}

[DebuggerStepThrough]
public sealed class FinallyBuilder : BlockOfCode
{
    internal FinallyBuilder(BlockOfCodeBase parent)
        : base(parent) =>
        Enabled = false;

    protected override void AppendHeader(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).AppendLine("finally");
}
