using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using MGen.Abstractions.Generators.Extensions.DotNetSerialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Shouldly;

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
    public (EmitResult EmitResult, Dictionary<string, GeneratedSourceResult> Sources, Assembly? Assembly) Compile(params Assembly[] extensionAssemblies)
    {
        var options = new CSharpParseOptions(
            LanguageVersion.Preview,
            DocumentationMode.Diagnose);

        var code = Source.ToString();

        TestContext.Out.WriteLine("Input:");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine(code);

        var compilation = CSharpCompilation.Create(
            assemblyName: "Test",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(code, options)
            },
            references: new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "netstandard.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.ComponentModel.TypeConverter.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.ObjectModel.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(GenerateAttribute).GetTypeInfo().Assembly.Location)
            },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(OptimizationLevel.Release)
                .WithNullableContextOptions(NullableContextOptions.Enable));

        var driver = CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(this),
                additionalTexts: extensionAssemblies.Select(assembly => new AssemblyExtension(assembly)).ToArray(),
                parseOptions: options,
                optionsProvider: new TestAnalyzerConfigOptionsProvider())
            .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);

        var runResults = driver.GetRunResult();

        var sources = runResults.Results
            .SelectMany(it => it.GeneratedSources)
            .ToDictionary(it => it.HintName);

        using var pdbStream = new MemoryStream();
        using var peStream = new MemoryStream();

        var emitResult = updatedCompilation.Emit(peStream, pdbStream);

        var assembly = emitResult.Success ? Assembly.Load(peStream.ToArray(), pdbStream.ToArray()) : null;

        return (emitResult, sources, assembly);
    }

    public static string Compile(params string[] lines)
    {
        var testModelGenerator = new TestModelGenerator(lines);

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile()
            .EmitResult
            .Diagnostics
            .Where(it => it.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();

        contents.ShouldNotBeNull();
        return contents;
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