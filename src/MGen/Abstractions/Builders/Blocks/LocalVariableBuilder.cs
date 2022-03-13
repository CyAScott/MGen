using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static LocalVariableBuilder AddVariable(this BlockOfCodeBase parent, string name, Code initialValue) => parent
        .Add(new LocalVariableBuilder(parent, "var", name, initialValue));

    [DebuggerStepThrough]
    public static LocalVariableBuilder AddVariable(this BlockOfCodeBase parent, string type, string name, Code? initialValue = null) => parent
        .Add(new LocalVariableBuilder(parent, type, name, initialValue));
}

[DebuggerStepThrough]
public class LocalVariableBuilder : IAmIndentedCode, IHaveAName
{
    internal LocalVariableBuilder(IAmIndentedCode? parent, string type, string name, Code? initialValue = null)
    {
        IndentLevel = parent == null ? 0 : parent.IndentLevel + 1;
        InitialValue = initialValue;
        Name = name;
        Parent = parent;
        Type = type;
    }

    public Code? InitialValue { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode? Parent { get; }

    public int IndentLevel { get; }

    public string Name { get; }

    public string Type { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        stringBuilder
            .AppendIndent(IndentLevel)
            .Append(Type).Append(' ').Append(Name);

        if (InitialValue != null)
        {
            stringBuilder.Append(" = ").AppendCode(InitialValue);
        }

        stringBuilder.AppendLine(";");
    }
}
