using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Components;
using System.Text;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveFields : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static FieldBuilder AddField(this IHaveFields members, string type, string name) => members
        .Add(new FieldBuilder(members, type, name));
}

[DebuggerStepThrough]
public class FieldBuilder :
    IHaveAReturnType,
    IHaveAttributes,
    IHaveAName,
    IHaveEnabled,
    IHaveModifiers,
    IHaveState,
    IHaveXmlComments
{
    internal FieldBuilder(IHaveFields parent, Code type, string name)
    {
        Attributes = new(this, true);
        XmlComments = new(this);
        IndentLevel = parent.IndentLevel + 1;
        Modifiers = new(Modifier.Internal, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Readonly, Modifier.Static, Modifier.Volatile);
        Name = name;
        Parent = parent;
        ReturnType = type;
    }

    public Components.Attributes Attributes { get; }

    public Code? Initializer { get; set; }

    public Code ReturnType { get; set; }

    public IAmIndentedCode Parent { get; }

    public Modifiers Modifiers { get; }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public XmlCommentsBuilder XmlComments { get; }

    public bool Enabled { get; set; } = true;

    public int IndentLevel { get; }

    public string Name { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        if (!Enabled)
        {
            return;
        }

        stringBuilder.AppendCode(XmlComments);

        stringBuilder.AppendCode(Attributes);

        stringBuilder.AppendIndent(IndentLevel);

        Modifiers.AppendModifiers(stringBuilder);

        stringBuilder.AppendCode(ReturnType).Append(' ').Append(Name);

        if (Initializer != null)
        {
            stringBuilder.Append(" = ").AppendCode(Initializer);
        }

        stringBuilder.AppendLine(";");
    }
}