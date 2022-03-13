using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Builders;

public interface IHaveClasses : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static ClassBuilder AddClass(this IHaveClasses members, string name, ITypeSymbol? inheritedTypeSymbol = null, SyntaxTokenList? modifiers = null) => members
        .Add(new ClassBuilder(members, name, inheritedTypeSymbol, modifiers));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#152-class-declarations">
/// A class
/// </see>
/// </summary>
[DebuggerStepThrough]
public sealed class ClassBuilder : BlockOfMembers,
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
    internal ClassBuilder(IHaveClasses parent, string name, ITypeSymbol? inheritedTypeSymbol = null, SyntaxTokenList? modifiers = null)
        : base(parent.IndentLevel + 1)
    {
        Attributes = new(this, true, inheritedTypeSymbol);
        XmlComments = new(this, inheritedTypeSymbol);
        GenericParameters = new(this, (inheritedTypeSymbol as INamedTypeSymbol)?.TypeArguments);
        Inheritance = new(this, inheritedTypeSymbol);
        Modifiers = parent is NamespaceBuilder ?
            new(modifiers, Modifier.Abstract, Modifier.Internal, Modifier.Partial, Modifier.Public, Modifier.Sealed, Modifier.Static) :
            new(modifiers, Modifier.Abstract, Modifier.Internal, Modifier.Partial, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Sealed, Modifier.Static);
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

    public string Keyword => "class";

    public string Name { get; }
}