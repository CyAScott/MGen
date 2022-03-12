using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Components;
using System.Text;

namespace MGen.Abstractions.Builders;

public interface IHaveDelegates : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    public static DelegateBuilder AddDelegate(this IHaveDelegates members, string returnType, string name) => members
        .Add(new DelegateBuilder(members, returnType, name));
}

public class DelegateBuilder :
    IHaveADeclarationKeyword,
    IHaveAName,
    IHaveAReturnType,
    IHaveArgumentParameters,
    IHaveAttributes,
    IHaveEnabled,
    IHaveGenericParameters,
    IHaveModifiers
{
    internal DelegateBuilder(IHaveDelegates parent, string returnType, string name)
    {
        ArgumentParameters = new(this);
        Attributes = new(this, true);
        XmlComments = new(this);
        GenericParameters = new(this);
        IndentLevel = parent.IndentLevel + 1;
        Modifiers = parent is NamespaceBuilder ?
            new(Modifier.Internal, Modifier.Public) :
            new(Modifier.Internal, Modifier.Private, Modifier.Protected, Modifier.Public);
        Name = name;
        Parent = parent;
        ReturnType = returnType;
    }

    public ArgumentParameters ArgumentParameters { get; }

    public Components.Attributes Attributes { get; }

    public Code ReturnType { get; set; }

    public GenericParameters GenericParameters { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode Parent { get; }

    public Modifiers Modifiers { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public bool ArgumentsEnabled => true;

    public bool Enabled { get; set; } = true;

    public int IndentLevel { get; }

    public string Keyword => "delegate";

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
        stringBuilder.Append(Keyword).Append(' ').AppendCode(ReturnType).Append(' ').Append(Name);

        GenericParameters.AppendParameters(stringBuilder);

        ArgumentParameters.AppendArguments(stringBuilder);

        if (GenericParameters.HasConstraints)
        {
            stringBuilder.AppendLine();
            GenericParameters.AppendConstraints(stringBuilder);
        }

        stringBuilder.AppendLine(";");
    }
}