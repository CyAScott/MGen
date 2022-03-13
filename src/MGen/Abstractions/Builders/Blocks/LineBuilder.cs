using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static BlockOfCodeBase AddEmptyLine(this BlockOfCodeBase parent)
    {
        parent.Add(Code.NewLine);
        return parent;
    }

    [DebuggerStepThrough]
    public static BlockOfCodeBase AddLine(this BlockOfCodeBase parent, Code line)
    {
        parent.Add(new LineBuilder(parent, line));
        return parent;
    }

    [DebuggerStepThrough]
    public static BlockOfCodeBase Return(this BlockOfCodeBase parent)
    {
        parent.Add(new LineBuilder(parent, "return"));
        return parent;
    }

    [DebuggerStepThrough]
    public static BlockOfCodeBase Return(this BlockOfCodeBase parent, Code value)
    {
        parent.Add(new LineBuilder(parent, new(sb => sb.Append("return ").AppendCode(value))));
        return parent;
    }

    [DebuggerStepThrough]
    public static BlockOfCodeBase Set(this BlockOfCodeBase parent, Code variable, Code value)
    {
        parent.Add(new LineBuilder(parent, new(sb => sb.AppendCode(variable).Append(" = ").AppendCode(value))));
        return parent;
    }
}

[DebuggerStepThrough]
public class LineBuilder : IAmIndentedCode
{
    internal LineBuilder(IAmIndentedCode parent, Code line, char? eol = ';')
    {
        Eol = eol;
        IndentLevel = parent.IndentLevel + 1;
        Line = line;
    }

    public Code Line { get; }

    public char? Eol { get; }

    public int IndentLevel { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        stringBuilder.AppendIndent(IndentLevel).AppendCode(Line);
        if (Eol != null)
        {
            stringBuilder.Append(Eol.Value);
        }
        stringBuilder.AppendLine();
    }
}
