using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MGen.Abstractions.Attributes;
using MGen.Abstractions.Builders;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MGen.Abstractions.Generators;

public class FileGenerator : IHaveState
{
    internal static bool TryToCreate(GeneratorContext context, Candidate candidate, CodeGenerators codeGenerators, out FileGenerator generator)
    {
        if (candidate.Member is not TypeDeclarationSyntax typeDeclarationSyntax)
        {
            generator = default!;
            return false;
        }

        var (@namespace, path, filePath) = candidate.GetFullyQualifiedMetadataName();

        var symbol = context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(path);
        if (symbol == null)
        {
            context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "MG_Type_0001",
                    "Unable to resolve type",
                    "Unable to resolve: {0}",
                    "CompileError",
                    DiagnosticSeverity.Error,
                    true), typeDeclarationSyntax.GetLocation(), path));
            generator = default!;
            return false;
        }

        if (candidate.Types.Count > 1 &&
            !candidate.Types.Take(candidate.Types.Count - 1).All(type => type.Modifiers.Any(it => it.ValueText == "partial")))
        {
            context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "MG_Type_0002",
                    "Unable to generate code for type because it is nested in a type that is not partial.",
                    "Unable to generate code: {0}",
                    "CompileError",
                    DiagnosticSeverity.Error,
                    true), typeDeclarationSyntax.GetLocation(), path));
            generator = default!;
            return false;
        }

        var attributes = symbol.GetMGenAttributes();

        var generateAttribute = attributes.OfType<GenerateAttributeRuntime>().FirstOrDefault();

        if (generateAttribute == null)
        {
            context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "MG_Type_0003",
                    "Unable to generate code for type because it does not have the GenerateAttribute on the declaration.",
                    "Unable to generate code: {0}",
                    "CompileError",
                    DiagnosticSeverity.Error,
                    true), typeDeclarationSyntax.GetLocation(), path));
            generator = default!;
            return false;
        }

        generator = new(
            candidate,
            generateAttribute,
            context,
            attributes,
            symbol,
            new(@namespace, codeGenerators),
            filePath);

        return true;
    }

    [DebuggerStepThrough]
    FileGenerator(
        Candidate candidate,
        GenerateAttributeRuntime generateAttribute,
        GeneratorContext generatorContext,
        IReadOnlyList<object> attributes,
        ITypeSymbol type,
        NamespaceBuilder builder,
        string filePath)
    {
        Attributes = attributes;
        Builder = builder;
        Candidate = candidate;
        FilePath = filePath;
        GenerateAttribute = generateAttribute;
        GeneratorContext = generatorContext;
        Type = type;
    }

    [ExcludeFromCodeCoverage]
    public Candidate Candidate { get; }

    public Dictionary<string, object> State { get; } = new();

    [ExcludeFromCodeCoverage]
    public GenerateAttributeRuntime GenerateAttribute { get; }

    [ExcludeFromCodeCoverage]
    public NamespaceBuilder Builder { get; }

    [ExcludeFromCodeCoverage]
    public GeneratorContext GeneratorContext { get; }

    [ExcludeFromCodeCoverage]
    public IReadOnlyList<object> Attributes { get; }

    [ExcludeFromCodeCoverage]
    public ITypeSymbol Type { get; }

    [ExcludeFromCodeCoverage]
    public string FilePath { get; }
}