using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MGen
{
    /// <summary>
    /// Scans the code base for <see cref="MemberDeclarationSyntax"/> with attributes.
    /// </summary>
    class AttributeSyntaxReceiver : ISyntaxReceiver
    {
        /// <summary>
        /// The path for the full name of the current member that is being scanned.
        /// This should contain the list of namespaces and class names where this member is located in the code.
        /// </summary>
        protected readonly List<string> Path = new();

        protected void ScanNode(SyntaxNode? node)
        {
            if (node == null)
            {
                return;
            }

            ScanNode(node.Parent);

            switch (node)
            {
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    Path.Add(namespaceDeclarationSyntax.Name.ToFullString().TrimEnd());
                    break;
                case ClassDeclarationSyntax classDeclarationSyntax:
                    Path.Add(classDeclarationSyntax.Identifier.Text.TrimEnd());
                    break;
            }
        }

        public AttributeSyntaxReceiver(params SyntaxKind[] syntaxKinds) => SyntaxKinds = new HashSet<SyntaxKind>(syntaxKinds);

        public Dictionary<string, MemberDeclarationSyntax> Candidates { get; } = new();

        public HashSet<SyntaxKind> SyntaxKinds { get; }

        void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not MemberDeclarationSyntax memberDeclarationSyntax ||
                memberDeclarationSyntax.AttributeLists.Count == 0 ||
                SyntaxKinds.Count > 0 && !SyntaxKinds.Contains(memberDeclarationSyntax.Kind()))
            {
                return;
            }

            ScanNode(memberDeclarationSyntax.Parent);

            if (memberDeclarationSyntax is BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
            {
                Path.Add(baseTypeDeclarationSyntax.Identifier.Text.TrimEnd());
            }

            Candidates.Add(string.Join(".", Path), memberDeclarationSyntax);

            Path.Clear();
        }
    }
}
