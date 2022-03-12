using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveUsings : IAmIndentedCode
{
    UsingsBuilder Usings { get; }
}

[DebuggerStepThrough]
public class UsingsBuilder : IAmCode, IHaveState, IReadOnlyCollection<Code>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _lines.GetEnumerator();

    readonly List<Code> _lines = new();

    internal UsingsBuilder(IHaveUsings parent) => Parent = parent;
    
    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public IHaveUsings Parent { get; }

    [ExcludeFromCodeCoverage]
    public IEnumerator<Code> GetEnumerator() => _lines.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _lines.Count;

    public void Add(Code code) => _lines.Add(code);

    public void Generate(StringBuilder stringBuilder)
    {
        if (Count == 0)
        {
            return;
        }

        foreach (var statement in this)
        {
            stringBuilder.AppendIndent(Parent.IndentLevel).Append("using ").AppendCode(statement).AppendLine(";");
        }

        stringBuilder.AppendIndent(Parent.IndentLevel).AppendLine();
    }
}