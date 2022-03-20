using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Components;
using System.Text;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveConstructors : IHaveTypes
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static ConstructorBuilder AddConstructor(this IHaveConstructors members) => members
        .Add(new ConstructorBuilder(members));
}

[DebuggerStepThrough]
public class ConstructorBuilder : BlockOfCode<IHaveConstructors>,
    IHaveAName,
    IHaveArgumentParameters,
    IHaveAttributes,
    IHaveModifiers,
    IHaveState
{
    internal ConstructorBuilder(IHaveConstructors parent)
        : base(parent, parent.IndentLevel + 1)
    {
        _parent = parent;
        ArgumentParameters = new(this);
        Attributes = new(this, true);
        XmlComments = new(this);
        Modifiers = new(Modifier.Internal, Modifier.Private, Modifier.Protected, Modifier.Public);
    }

    protected override void AppendHeader(StringBuilder stringBuilder)
    {
        stringBuilder.AppendCode(XmlComments);

        stringBuilder.AppendCode(Attributes);

        stringBuilder.AppendIndent(IndentLevel);

        Modifiers.AppendModifiers(stringBuilder);

        stringBuilder.Append(Name);

        ArgumentParameters.AppendArguments(stringBuilder);

        stringBuilder.AppendLine();
    }

    public ArgumentParameters ArgumentParameters { get; }

    public Components.Attributes Attributes { get; }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public Modifiers Modifiers { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public bool ArgumentsEnabled => true;

    public string Name => _parent.Name;

    public void GenerateCode() => _parent.CodeGenerators.GenerateCode(this);

    readonly IHaveConstructors _parent;
}