using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Builders.Components;

public interface IHaveInheritance : IAmIndentedCode
{
    InheritanceBuilder Inheritance { get; }
}

[DebuggerStepThrough]
public class InheritanceBuilder : IAmCode, IHaveState, IReadOnlyCollection<Code>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    readonly List<Code> _items = new();

    internal InheritanceBuilder(IHaveInheritance parent, ITypeSymbol? inheritedTypeSymbol = null)
    {
        Parent = parent;
        if (inheritedTypeSymbol != null)
        {
            Add(new CodeWithInheritedTypeSymbol(inheritedTypeSymbol));
        }
    }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public IHaveInheritance Parent { get; }

    [ExcludeFromCodeCoverage]
    public IEnumerator<Code> GetEnumerator() => _items.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _items.Count;

    public void Add(Code item) => _items.Add(item);

    public void Generate(StringBuilder stringBuilder)
    {
        var isFirst = true;
        foreach (var item in this)
        {
            if (isFirst)
            {
                isFirst = false;
                stringBuilder.Append(" : ");
            }
            else
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.AppendCode(item);
        }
    }
}

[DebuggerStepThrough]
public class CodeWithInheritedTypeSymbol : Code
{
    public CodeWithInheritedTypeSymbol(ITypeSymbol inheritedTypeSymbol)
        : base(stringBuilder => AddInheritedTypeSymbol(stringBuilder, inheritedTypeSymbol, (inheritedTypeSymbol as INamedTypeSymbol)?.TypeArguments)) =>
        InheritedTypeSymbol = inheritedTypeSymbol;

    public ITypeSymbol InheritedTypeSymbol { get; }

    static void AddInheritedTypeSymbol(StringBuilder stringBuilder, ISymbol inheritedTypeSymbol, ImmutableArray<ITypeSymbol>? genericParameters)
    {
        stringBuilder.Append(inheritedTypeSymbol.Name);

        if (genericParameters == null || genericParameters.Value.IsEmpty)
        {
            return;
        }

        stringBuilder.Append('<');

        var isFirst = true;

        foreach (var parameter in genericParameters)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(parameter.Name);
        }

        stringBuilder.Append('>');
    }
}