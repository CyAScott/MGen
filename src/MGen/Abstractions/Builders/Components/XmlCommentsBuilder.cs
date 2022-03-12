using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Builders.Components;

public interface IHaveXmlComments : IAmIndentedCode
{
    XmlCommentsBuilder XmlComments { get; }
}

[DebuggerStepThrough]
public partial class XmlCommentsBuilder : IAmCode, IReadOnlyCollection<string>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _summary.GetEnumerator();

    readonly List<string> _summary = new();

    internal XmlCommentsBuilder(IHaveXmlComments parent, ISymbol? symbol = null)
    {
        Parent = parent;

        var comments = symbol?.GetDocumentationCommentXml();

        if (!string.IsNullOrEmpty(comments))
        {
            Init(comments.AsSpan());
        }
    }

    public IHaveXmlComments Parent { get; }

    [ExcludeFromCodeCoverage]
    public IEnumerator<string> GetEnumerator() => _summary.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _summary.Count;

    public void Add(string code) => _summary.Add(code);

    public Dictionary<string, List<string>> Exceptions { get; } = new();

    public Dictionary<string, List<string>> Parameters { get; } = new();

    public Dictionary<string, List<string>> TypeParameters { get; } = new();

    public List<string> Remarks { get; } = new();

    public List<string> Returns { get; } = new();

    public List<string> Value { get; } = new();

    void Init(ReadOnlySpan<char> comments)
    {
        var cref = "cref".AsSpan();
        var exception = "exception".AsSpan();
        var name = "name".AsSpan();
        var param = "param".AsSpan();
        var remarks = "remarks".AsSpan();
        var returns = "returns".AsSpan();
        var summary = "summary".AsSpan();
        var typeparam = "typeparam".AsSpan();
        var value = "value".AsSpan();

        var buffer = comments;

        while (ElementInfo.Next(buffer, out var elementInfo))
        {
            if (elementInfo.Name.Equals(exception, StringComparison.Ordinal))
            {
                var attributeValue = elementInfo.GetAttribute(cref);
                if (attributeValue != null)
                {
                    var lines = Exceptions[attributeValue] = new();
                    elementInfo.CopyContentsTo(lines, out buffer);
                }
            }
            else if (elementInfo.Name.Equals(param, StringComparison.Ordinal))
            {
                var attributeValue = elementInfo.GetAttribute(name);
                if (attributeValue != null)
                {
                    var lines = Parameters[attributeValue] = new();
                    elementInfo.CopyContentsTo(lines, out buffer);
                }
            }
            else if (elementInfo.Name.Equals(remarks, StringComparison.Ordinal))
            {
                elementInfo.CopyContentsTo(Remarks, out buffer);
            }
            else if (elementInfo.Name.Equals(returns, StringComparison.Ordinal))
            {
                elementInfo.CopyContentsTo(Returns, out buffer);
            }
            else if (elementInfo.Name.Equals(summary, StringComparison.Ordinal))
            {
                elementInfo.CopyContentsTo(_summary, out buffer);
            }
            else if (elementInfo.Name.Equals(typeparam, StringComparison.Ordinal))
            {
                var attributeValue = elementInfo.GetAttribute(name);
                if (attributeValue != null)
                {
                    var lines = TypeParameters[attributeValue] = new();
                    elementInfo.CopyContentsTo(lines, out buffer);
                }
            }
            else if (elementInfo.Name.Equals(value, StringComparison.Ordinal))
            {
                elementInfo.CopyContentsTo(Value, out buffer);
            }
            else
            {
                buffer = elementInfo.After;
            }
        }
    }
}
