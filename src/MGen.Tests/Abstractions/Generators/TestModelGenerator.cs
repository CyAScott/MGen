using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using MGen.Abstractions.Generators.Extensions;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MGen.Abstractions.Generators;

[MGenExtension(nameof(TestModelGenerator), after: new[] { DotNetSerializationSupport.Id }), DebuggerStepThrough]
class TestModelGenerator : ModelGenerator,
    IHandleOnInit,
    IHandleOnTypeGenerated,
    IHandleOnTypesGenerated,
    IHandleOnFileGenerated,
    IHandleOnFilesGenerated
{
    public TestModelGenerator(params string[] lines)
    {
        foreach (var line in lines)
        {
            Source.AppendLine(line);
        }
    }

    static readonly string dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
    public Compilation Compile(out ImmutableArray<Diagnostic> diagnostics)
    {
        var options = new CSharpParseOptions(
            LanguageVersion.Preview,
            DocumentationMode.Diagnose);

        var code = Source.ToString();

        var compilation = CSharpCompilation.Create(
            assemblyName: "Test",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(code, options)
            },
            references: new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(GenerateAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(INotifyCollectionChanged).GetTypeInfo().Assembly.Location)
            },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(OptimizationLevel.Release));

        CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(this),
                additionalTexts: ImmutableArray<AdditionalText>.Empty,
                parseOptions: options,
                optionsProvider: null)
            .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);

        return updatedCompilation;
    }

    public StringBuilder Source { get; } = new();

    public event Action<InitArgs>? Init;
    void IHandleOnInit.Init(InitArgs args) => Init?.Invoke(args);

    public event Action<FileGeneratedArgs>? FileGenerated;
    void IHandleOnFileGenerated.FileGenerated(FileGeneratedArgs args) => FileGenerated?.Invoke(args);

    public event Action<FilesGeneratedArgs>? FilesGenerated;
    void IHandleOnFilesGenerated.FilesGenerated(FilesGeneratedArgs args) => FilesGenerated?.Invoke(args);

    public event Action<TypeGeneratedArgs>? TypeGenerated;
    void IHandleOnTypeGenerated.TypeGenerated(TypeGeneratedArgs args) => TypeGenerated?.Invoke(args);

    public event Action<TypesGeneratedArgs>? TypesGenerated;
    void IHandleOnTypesGenerated.TypesGenerated(TypesGeneratedArgs args) => TypesGenerated?.Invoke(args);
}