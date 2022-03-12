using MGen.Abstractions.Builders.Members;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MGen.Abstractions.Builders;

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations">
/// A namespace
/// </see>
/// </summary>
public class NamespaceBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveClasses,
    IHaveDelegates,
    IHaveInterfaces,
    IHaveRecords,
    IHaveStructs,
    IHaveUsings
{
    internal NamespaceBuilder(NamespaceBuilder parent, string name)
        : base(parent.IndentLevel + 1)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Parent = parent;
        Usings = new(this);
    }

    internal NamespaceBuilder(string name)
        : base(0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Usings = new(this);
    }

    public NamespaceBuilder AddNamespace(string name) => Add(new NamespaceBuilder(this, name));

    [ExcludeFromCodeCoverage]
    public NamespaceBuilder? Parent { get; }

    public UsingsBuilder Usings { get; }

    public string Keyword => "namespace";

    public string Name { get; }
}