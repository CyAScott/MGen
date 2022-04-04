using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static LocalVariableBuilder AddVariable(this BlockOfCodeBase parent, Code name, Code initialValue) => parent
        .Add(new LocalVariableBuilder(parent, "var", name, initialValue));

    [DebuggerStepThrough]
    public static LocalVariableBuilder AddVariable(this BlockOfCodeBase parent, string type, Code name, Code? initialValue = null) => parent
        .Add(new LocalVariableBuilder(parent, type, name, initialValue));
}

[DebuggerStepThrough]
public class LocalVariableBuilder : IAmIndentedCode
{
    internal LocalVariableBuilder(IAmIndentedCode? parent, string type, Code name, Code? initialValue = null)
    {
        IndentLevel = parent == null ? 0 : parent.IndentLevel + 1;
        InitialValue = initialValue;
        Name = name;
        Parent = parent;
        Type = type;
    }

    public Code? InitialValue { get; }

    public Code Name { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode? Parent { get; }

    public int IndentLevel { get; }

    public string Type { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        stringBuilder
            .AppendIndent(IndentLevel)
            .Append(Type).Append(' ').AppendCode(Name);

        if (InitialValue != null)
        {
            stringBuilder.Append(" = ").AppendCode(InitialValue);
        }

        stringBuilder.AppendLine(";");
    }
}
