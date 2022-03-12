using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MGen.Abstractions.Generators;

/// <summary>
/// Scans the code base for <see cref="MemberDeclarationSyntax"/> with attributes.
/// </summary>
class AttributeSyntaxReceiver : ISyntaxReceiver
{
    [DebuggerStepThrough]
    public AttributeSyntaxReceiver(params SyntaxKind[] syntaxKinds) => SyntaxKinds = new HashSet<SyntaxKind>(syntaxKinds);

    public List<Candidate> Candidates { get; } = new();

    public HashSet<SyntaxKind> SyntaxKinds { get; }

    void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is MemberDeclarationSyntax { AttributeLists.Count: > 0 } memberDeclarationSyntax &&
            (SyntaxKinds.Count == 0 || SyntaxKinds.Contains(memberDeclarationSyntax.Kind())))
        {
            var candidate = new Candidate(memberDeclarationSyntax);
            Candidates.Add(candidate);
        }
    }
}

[DebuggerStepThrough]
public class Candidate
{
    public Candidate(MemberDeclarationSyntax member)
    {
        Member = member;

        ScanNode(member.Parent);

        if (Member is TypeDeclarationSyntax type)
        {
            _types.Add(type);
        }
    }

    [ExcludeFromCodeCoverage]
    public IReadOnlyList<TypeDeclarationSyntax> Types => _types;
    readonly List<TypeDeclarationSyntax> _types = new();

    [ExcludeFromCodeCoverage]
    public IReadOnlyList<NamespaceDeclarationSyntax> Namespaces => _namespaces;
    readonly List<NamespaceDeclarationSyntax> _namespaces = new();

    public MemberDeclarationSyntax Member { get; }

    /// <summary>
    /// Gets the a fully qualified metadata name used to get the <see cref="INamedTypeSymbol"/> for this candidate.
    /// </summary>
    public (string Namespace, string Path, string FilePath) GetFullyQualifiedMetadataName()
    {
        var @namespace = string.Join(".", _namespaces.Select(it => it.Name.ToFullString().TrimEnd()));

        var filePath = new List<string>
        {
            @namespace
        };

        var path = new StringBuilder(@namespace).Append('.');

        for (var index = 0; index < _types.Count - 1; index++)
        {
            var type = _types[index];

            var name = type.Identifier.Text.TrimEnd();

            filePath.Add(name);
            path.Append(name);

            if (type.TypeParameterList is { Parameters.Count: > 0 })
            {
                path.Append('`').Append(type.TypeParameterList.Parameters.Count);
            }

            path.Append('+');
        }

        var @interface = _types.Last();

        var interfaceName = @interface.Identifier.Text.TrimEnd();

        filePath.Add(interfaceName);
        path.Append(interfaceName);

        if (@interface.TypeParameterList is { Parameters.Count: > 0 })
        {
            path.Append('`').Append(@interface.TypeParameterList.Parameters.Count);
        }

        return (@namespace, path.ToString(), string.Join(".", filePath) + ".cs");
    }

    void ScanNode(SyntaxNode? node)
    {
        if (node == null)
        {
            return;
        }

        ScanNode(node.Parent);

        switch (node)
        {
            case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                _namespaces.Add(namespaceDeclarationSyntax);
                break;
            case TypeDeclarationSyntax typeDeclarationSyntax:
                _types.Add(typeDeclarationSyntax);
                break;
        }
    }
}