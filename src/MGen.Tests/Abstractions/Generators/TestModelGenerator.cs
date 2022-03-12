using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MGen.Abstractions.Generators.Extensions;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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
    public Compilation Compile(out ImmutableArray<Diagnostic> diagnostics) => Compile(Array.Empty<Assembly>(), out diagnostics);
    public Compilation Compile(Assembly[] extensionAssemblies, out ImmutableArray<Diagnostic> diagnostics)
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
                additionalTexts: extensionAssemblies.Select(assembly => new AssemblyExtension(assembly)).ToArray(),
                parseOptions: options,
                optionsProvider: new TestAnalyzerConfigOptionsProvider())
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

[DebuggerStepThrough]
class AssemblyExtension : AdditionalText
{
    readonly Assembly _assembly;

    public AssemblyExtension(Assembly assembly)
    {
        _assembly = assembly;
        Options.Options.Add("build_metadata.additionalfiles.MGenExtension", "true");
    }

    public TestAnalyzerConfigOptions Options { get; } = new();

    public override SourceText GetText(CancellationToken cancellationToken = default) => SourceText.From(Path);

    public override string Path => _assembly.Location;
}

[DebuggerStepThrough]
class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public Dictionary<string, string> Options { get; } = new();

    public override bool TryGetValue(string key, out string value) => Options.TryGetValue(key, out value);

    public static readonly AnalyzerConfigOptions Empty = new TestAnalyzerConfigOptions();
}

[DebuggerStepThrough]
class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    public override AnalyzerConfigOptions GlobalOptions => TestAnalyzerConfigOptions.Empty;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => TestAnalyzerConfigOptions.Empty;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
        textFile is AssemblyExtension assemblyExtension
            ? assemblyExtension.Options
            : TestAnalyzerConfigOptions.Empty;
}