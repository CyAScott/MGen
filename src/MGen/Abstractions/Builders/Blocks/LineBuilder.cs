using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    public static LineBuilder AddLine(this BlockOfCodeBase parent, Code line) => parent
        .Add(new LineBuilder(parent, line));

    public static LineBuilder Return(this BlockOfCodeBase parent) => parent
        .Add(new LineBuilder(parent, "return"));

    public static LineBuilder Return(this BlockOfCodeBase parent, Code value) => parent
        .Add(new LineBuilder(parent, new(sb => sb.Append("return ").AppendCode(value))));

    public static LineBuilder Set(this BlockOfCodeBase parent, Code variable, Code value) => parent
        .Add(new LineBuilder(parent, new(sb => sb.AppendCode(variable).Append(" = ").AppendCode(value))));
}

public class LineBuilder : IAmIndentedCode
{
    internal LineBuilder(IAmIndentedCode parent, Code line)
    {
        IndentLevel = parent.IndentLevel + 1;
        Line = line;
    }

    public Code Line { get; }

    public int IndentLevel { get; }

    public void Generate(StringBuilder stringBuilder) =>
        stringBuilder.AppendIndent(IndentLevel).AppendCode(Line).AppendLine(";");
}
