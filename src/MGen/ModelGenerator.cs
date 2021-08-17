using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MGen
{
    [Generator]
    class ModelGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not AttributeSyntaxReceiver receiver ||
                receiver.Candidates.Count == 0)
            {
                return;
            }

            //NOTE: currently MGen is only scanning for interfaces
            var interfaces = new List<InterfaceInfo>();

            foreach (var type in receiver.Candidates)
            {
                if (type.Value is not InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                {
                    continue;
                }

                var path = type.Key;

                var interfaceSymbol = context.Compilation.GetTypeByMetadataName(
                    interfaceDeclarationSyntax.TypeParameterList?.Parameters.Count is null or 0 ? path : path + "`" + interfaceDeclarationSyntax.TypeParameterList.Parameters.Count);
                if (interfaceSymbol == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "MG0001",
                            "Unable to resolve interface",
                            "Unable to resolve: {0}",
                            "CompileError",
                            DiagnosticSeverity.Warning,
                        true), interfaceDeclarationSyntax.GetLocation(), type.Key));
                    continue;
                }

                var attributes = interfaceSymbol.GetMGenAttributes();
                if (attributes.Count == 0)
                {
                    continue;
                }

                interfaces.Add(new InterfaceInfo(
                    path,
                    interfaceSymbol,
                    interfaceDeclarationSyntax.Modifiers,
                    attributes));
            }

            foreach (var (fullName, code) in interfaces.GenerateCode(context))
            {
                var filePath = fullName + ".cs";

                context.AddSource(filePath, code);
            }
        }

        public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() =>
            new AttributeSyntaxReceiver(SyntaxKind.InterfaceDeclaration));
    }
}
