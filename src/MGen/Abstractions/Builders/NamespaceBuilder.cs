using MGen.Abstractions.Builders.Members;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Builders;

/// <summary>
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations">
/// A namespace
/// </see>
/// </summary>
[DebuggerStepThrough]
public class NamespaceBuilder : BlockOfMembers,
    IHaveADeclarationKeyword,
    IHaveDelegates,
    IHaveTypes,
    IHaveUsings
{
    internal NamespaceBuilder(NamespaceBuilder parent, string name)
        : base(parent.IndentLevel + 1)
    {
        CodeGenerators = parent.CodeGenerators;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Parent = parent;
        Usings = new(this);
    }

    internal NamespaceBuilder(string name, CodeGenerators? codeGenerators = null)
        : base(0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CodeGenerators = codeGenerators ?? new();
        Usings = new(this);
    }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public CodeGenerators CodeGenerators { get; }

    public NamespaceBuilder AddNamespace(string name) => Add(new NamespaceBuilder(this, name));

    [ExcludeFromCodeCoverage]
    public NamespaceBuilder? Parent { get; }

    public UsingsBuilder Usings { get; }

    public string Keyword => "namespace";

    public string Name { get; }
}