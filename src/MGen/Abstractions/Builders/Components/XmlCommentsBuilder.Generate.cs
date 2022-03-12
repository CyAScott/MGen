using System.Collections.Generic;
using System.Text;

namespace MGen.Abstractions.Builders.Components;

public partial class XmlCommentsBuilder
{
    void Append(StringBuilder stringBuilder, string tagName, List<string> lines, bool allowSingleLine = true)
    {
        if (allowSingleLine && lines.Count == 1)
        {
            stringBuilder.AppendIndent(Parent.IndentLevel)
                .Append("/// <").Append(tagName).Append(">")
                .Append(lines[0])
                .Append("</").Append(tagName).AppendLine(">");
        }
        else if (lines.Count > 0)
        {
            stringBuilder.AppendIndent(Parent.IndentLevel).Append("/// <").Append(tagName).AppendLine(">");
            foreach (var line in lines)
            {
                stringBuilder.AppendIndent(Parent.IndentLevel).Append("/// ").Append(line).AppendLine();
            }
            stringBuilder.AppendIndent(Parent.IndentLevel).Append("/// </").Append(tagName).AppendLine(">");
        }
    }

    void Append(StringBuilder stringBuilder, string tagName, Dictionary<string, List<string>> items, string attributeLabel = "name")
    {
        if (items.Count > 0)
        {
            foreach (var pair in items)
            {
                var key = pair.Key;
                var lines = pair.Value;

                if (lines.Count == 1)
                {
                    stringBuilder.AppendIndent(Parent.IndentLevel)
                        .Append("/// <").Append(tagName).Append(' ').Append(attributeLabel).Append("=\"").Append(key).Append("\">")
                        .Append(lines[0])
                        .Append("</").Append(tagName).AppendLine(">");
                }
                else if (lines.Count > 0)
                {
                    stringBuilder.AppendIndent(Parent.IndentLevel).Append("/// <").Append(tagName).Append(' ').Append(attributeLabel).Append("=\"").Append(key).AppendLine("\">");
                    foreach (var line in lines)
                    {
                        stringBuilder.AppendIndent(Parent.IndentLevel).Append("/// ").Append(line).AppendLine();
                    }
                    stringBuilder.AppendIndent(Parent.IndentLevel).Append("/// </").Append(tagName).AppendLine(">");
                }
            }
        }
    }

    public void Generate(StringBuilder stringBuilder)
    {
        Append(stringBuilder, "summary", _summary, false);

        Append(stringBuilder, "remarks", Remarks);

        Append(stringBuilder, "value", Value);

        Append(stringBuilder, "exception", Exceptions, "cref");

        Append(stringBuilder, "param", Parameters);

        Append(stringBuilder, "typeparam", TypeParameters);

        Append(stringBuilder, "returns", Returns);
    }
}