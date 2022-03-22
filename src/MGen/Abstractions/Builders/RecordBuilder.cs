using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Builders;

public interface IHaveRecords : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static RecordBuilder AddRecord(this IHaveTypes members, string name, ITypeSymbol? inheritedTypeSymbol = null, SyntaxTokenList? modifiers = null) =>
        members.Add(new RecordBuilder(members, name, inheritedTypeSymbol, modifiers));
}

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/records">
/// A record
/// </see>
/// </summary>
[DebuggerStepThrough]
public sealed class RecordBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveAStaticConstructor,
    IHaveAttributes,
    IHaveDelegates,
    IHaveEvents,
    IHaveFields,
    IHaveGenericParameters,
    IHaveInheritance,
    IHaveMethods,
    IHaveProperties,
    IInvokeCodeGenerators
{
    internal RecordBuilder(IHaveTypes parent, string name, ITypeSymbol? inheritedTypeSymbol = null, SyntaxTokenList? modifiers = null)
        : base(parent.IndentLevel + 1)
    {
        Attributes = new(this, true, inheritedTypeSymbol);
        XmlComments = new(this, inheritedTypeSymbol);
        GenericParameters = new(this, (inheritedTypeSymbol as INamedTypeSymbol)?.TypeArguments);
        CodeGenerators = parent.CodeGenerators;
        Inheritance = new(this, inheritedTypeSymbol);
        Modifiers = parent is NamespaceBuilder ?
            new(modifiers, Modifier.Abstract, Modifier.Internal, Modifier.Partial, Modifier.Public, Modifier.Static) :
            new(modifiers, Modifier.Abstract, Modifier.Internal, Modifier.Partial, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Static);
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
    public Dictionary<string, object> State { get; } = new();

    public GenericParameters GenericParameters { get; }

    public CodeGenerators CodeGenerators { get; }

    public IHaveTypes Parent { get; }

    public Modifiers Modifiers { get; }

    public InheritanceBuilder Inheritance { get; }

    public StaticConstructorBuilder StaticConstructor { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public string GetFullPath(bool includeSelf = true)
    {
        if (!includeSelf)
        {
            return Parent.GetFullPath();
        }

        if (GenericParameters.Count == 0)
        {
            return Parent.GetFullPath() + "." + Name;
        }

        return Parent.GetFullPath() + "." + Name + "<" + string.Join(", ", GenericParameters.Keys) + ">";
    }

    public string Keyword => "record";

    public string Name { get; }

    public void GenerateCode() => Parent.CodeGenerators.TypeCreated(Parent, this);
}