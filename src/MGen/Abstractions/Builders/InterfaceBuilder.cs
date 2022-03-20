using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Builders;

public interface IHaveInterfaces : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static InterfaceBuilder AddInterface(this IHaveTypes members, string name)
    {
        var type = members.Add(new InterfaceBuilder(members, name));
        members.CodeGenerators.TypeCreated(members, type);
        return type;
    }
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/interfaces#182-interface-declarations">
/// An interface
/// </see>
/// </summary>
[DebuggerStepThrough]
public sealed class InterfaceBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveAStaticConstructor,
    IHaveAttributes,
    IHaveDelegates,
    IHaveEvents,
    IHaveGenericParameters,
    IHaveInheritance,
    IHaveMethods,
    IHaveProperties
{
    internal InterfaceBuilder(IHaveTypes parent, string name)
        : base(parent.IndentLevel + 1)
    {
        Attributes = new(this, true);
        XmlComments = new(this);
        GenericParameters = new(this);
        CodeGenerators = parent.CodeGenerators;
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
    public Dictionary<string, object> State { get; } = new();

    public GenericParameters GenericParameters { get; }

    public CodeGenerators CodeGenerators { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode Parent { get; }

    public Modifiers Modifiers { get; }

    public InheritanceBuilder Inheritance { get; }

    public StaticConstructorBuilder StaticConstructor { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public string Keyword => "interface";

    public string Name { get; }
}