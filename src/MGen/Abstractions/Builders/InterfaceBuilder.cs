using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MGen.Abstractions.Builders;

public interface IHaveInterfaces : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    public static InterfaceBuilder AddInterface(this IHaveInterfaces members, string name) => members
        .Add(new InterfaceBuilder(members, name));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/interfaces#182-interface-declarations">
/// An interface
/// </see>
/// </summary>
public sealed class InterfaceBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveAStaticConstructor,
    IHaveAttributes,
    IHaveClasses,
    IHaveDelegates,
    IHaveEvents,
    IHaveGenericParameters,
    IHaveInheritance,
    IHaveInterfaces,
    IHaveMethods,
    IHaveProperties,
    IHaveRecords,
    IHaveStructs
{
    internal InterfaceBuilder(IHaveInterfaces parent, string name)
        : base(parent.IndentLevel + 1)
    {
        Attributes = new(this, true);
        XmlComments = new(this);
        GenericParameters = new(this);
        Inheritance = new(this);
        Modifiers = parent is NamespaceBuilder ?
            new(Modifier.Internal, Modifier.Partial, Modifier.Public, Modifier.Static) :
            new(Modifier.Internal, Modifier.Partial, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Static);
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Parent = parent;

        Add(StaticConstructor = new(this));
    }

    public Components.Attributes Attributes { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode Parent { get; }

    public GenericParameters GenericParameters { get; }

    public Modifiers Modifiers { get; }

    public InheritanceBuilder Inheritance { get; }

    public StaticConstructorBuilder StaticConstructor { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public string Keyword => "interface";

    public string Name { get; }
}