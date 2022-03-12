using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions;

public abstract class CodeCollection : IAmIndentedCode, IHaveEnabled, IReadOnlyList<IAmCode>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _code.GetEnumerator();

    protected CodeCollection(int indentLevel) => IndentLevel = indentLevel;

    protected abstract void AppendHeader(StringBuilder stringBuilder);

    protected internal virtual bool IsBodyEnabled => true;

    protected virtual bool AppendBlankLinesBetweenItems => false;

    protected virtual void AppendBody(StringBuilder stringBuilder)
    {
        if (IsBodyEnabled)
        {
            stringBuilder.AppendIndent(IndentLevel).AppendLine("{");

            var count = 0;

            foreach (var item in this)
            {
                if (AppendBlankLinesBetweenItems && count > 0)
                {
                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendCode(item);

                if (AppendBlankLinesBetweenItems && (item is not IHaveEnabled hasEnabled || hasEnabled.Enabled))
                {
                    count++;
                }
            }

            stringBuilder.AppendIndent(IndentLevel).AppendLine("}");
        }
    }

    [ExcludeFromCodeCoverage]
    public IAmCode this[int index] => _code[index];

    [ExcludeFromCodeCoverage]
    public IEnumerator<IAmCode> GetEnumerator() => _code.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _code.Count;

    public int IndentLevel { get; }

    public virtual T Add<T>(T value)
        where T : IAmCode
    {
        _code.Add(value);
        return value;
    }

    public virtual bool Enabled { get; set; } = true;

    public virtual void Generate(StringBuilder stringBuilder)
    {
        if (Enabled)
        {
            AppendHeader(stringBuilder);
            AppendBody(stringBuilder);
        }
    }

    [ExcludeFromCodeCoverage]
    public void Clear() => _code.Clear();

    [ExcludeFromCodeCoverage]
    public void Remove(IAmCode item) => _code.Remove(item);

    [ExcludeFromCodeCoverage]
    public void Remove(int index) => _code.RemoveAt(index);

    readonly List<IAmCode> _code = new();
}