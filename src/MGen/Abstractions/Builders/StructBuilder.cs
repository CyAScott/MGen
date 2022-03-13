using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MGen.Abstractions.Builders;

public interface IHaveStructs : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static StructBuilder AddStruct(this IHaveStructs members, string name) => members
        .Add(new StructBuilder(members, name));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/structs#152-struct-declarations">
/// A struct
/// </see>
/// </summary>
[DebuggerStepThrough]
public sealed class StructBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveAStaticConstructor,
    IHaveAttributes,
    IHaveClasses,
    IHaveDelegates,
    IHaveGenericParameters,
    IHaveInheritance,
    IHaveInterfaces,
    IHaveMembersWithCode,
    IHaveRecords,
    IHaveStructs
{
    internal StructBuilder(IHaveStructs parent, string name)
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
        StaticConstructor = new(this)
        {
            Enabled = false
        };

        Add(StaticConstructor);
    }

    public Components.Attributes Attributes { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode Parent { get; }

    public GenericParameters GenericParameters { get; }

    public Modifiers Modifiers { get; }

    public InheritanceBuilder Inheritance { get; }

    public StaticConstructorBuilder StaticConstructor { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public string Keyword => "struct";

    public string Name { get; }
}