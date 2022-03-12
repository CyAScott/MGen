using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System;

namespace MGen.Abstractions.Builders;

public interface IHaveRecords : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    public static RecordBuilder AddRecord(this IHaveRecords members, string name) => members
        .Add(new RecordBuilder(members, name));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/records">
/// A record
/// </see>
/// </summary>
public sealed class RecordBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveAStaticConstructor,
    IHaveAttributes,
    IHaveClasses,
    IHaveDelegates,
    IHaveEvents,
    IHaveFields,
    IHaveGenericParameters,
    IHaveInheritance,
    IHaveInterfaces,
    IHaveMethods,
    IHaveProperties,
    IHaveRecords,
    IHaveStructs
{
    internal RecordBuilder(IHaveRecords parent, string name)
        : base(parent.IndentLevel + 1)
    {
        Attributes = new(this, true);
        XmlComments = new(this);
        GenericParameters = new(this);
        Inheritance = new(this);
        Modifiers = parent is NamespaceBuilder ?
            new(Modifier.Abstract, Modifier.Internal, Modifier.Partial, Modifier.Public, Modifier.Static) :
            new(Modifier.Abstract, Modifier.Internal, Modifier.Partial, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Static);
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Parent = parent;
        StaticConstructor = new(this)
        {
            Enabled = false
        };

        Add(StaticConstructor);
    }

    public Components.Attributes Attributes { get; }

    public IAmIndentedCode Parent { get; }

    public GenericParameters GenericParameters { get; }

    public Modifiers Modifiers { get; }

    public InheritanceBuilder Inheritance { get; }

    public StaticConstructorBuilder StaticConstructor { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public string Keyword => "record";

    public string Name { get; }
}