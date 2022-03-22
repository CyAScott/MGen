using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MGen.Abstractions.Generators;

[Generator, DebuggerStepThrough]
class ModelGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is AttributeSyntaxReceiver receiver)
        {
            var extensions = new ExtensionsCollection<IAmAMGenExtension>
            {
                typeof(ModelGenerator).Assembly
            };

            if (this is IAmAMGenExtension extension)
            {
                extensions.Add(extension);
            }

            extensions.Add(context);

            var generatorContext = new GeneratorContext(context, receiver.Candidates, extensions.ToSortedList());

            generatorContext.Generate();
        }
    }

    public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver(SyntaxKind.InterfaceDeclaration));
}

[DebuggerStepThrough]
public class GeneratorContext
{
    internal GeneratorContext(GeneratorExecutionContext generatorExecutionContext, List<Candidate> candidates, List<IAmAMGenExtension> extensions)
    {
        Candidates = candidates;
        Extensions = extensions;
        GeneratorExecutionContext = generatorExecutionContext;
    }

    internal void Generate()
    {
        var initArgs = new InitArgs(this);
        foreach (var extension in Extensions.OfType<IHandleOnInit>())
        {
            extension.Init(initArgs);
        }

        var codeGenerators = new CodeGenerators(this, CodeGenerators, Extensions.OfType<IHandleOnTypeCreated>().ToList());

        foreach (var candidate in Candidates)
        {
            codeGenerators.CurrentCandidate = candidate;

            if (FileGenerator.TryToCreate(this, candidate, codeGenerators, out var generator))
            {
                codeGenerators.CurrentFile = generator;

                _files.Add(generator);

                var fileCreatedArgs = new FileCreatedArgs(this, generator);
                foreach (var extension in Extensions.OfType<IHandleOnFileCreated>())
                {
                    extension.FileCreated(fileCreatedArgs);
                }
            }
        }

        codeGenerators.CurrentCandidate = null;
        codeGenerators.CurrentFile = null;

        var filesCreatedArgs = new FilesCreatedArgs(this);
        foreach (var extension in Extensions.OfType<IHandleOnFilesCreated>())
        {
            extension.FilesCreated(filesCreatedArgs);
        }

        var builder = new StringBuilder();

        foreach (var generator in _files)
        {
            if (generator.Builder.Enabled)
            {
                builder.AppendCode(generator.Builder);

                var contents = builder.ToString();

                var fileGeneratedArgs = new FileGeneratedArgs(this, generator, contents);
                foreach (var extension in Extensions.OfType<IHandleOnFileGenerated>())
                {
                    extension.FileGenerated(fileGeneratedArgs);
                }

                GeneratorExecutionContext.AddSource(generator.FilePath, contents);

                builder.Clear();
            }
        }

        var filesGeneratedArgs = new FilesGeneratedArgs(this);
        foreach (var extension in Extensions.OfType<IHandleOnFilesGenerated>())
        {
            extension.FilesGenerated(filesGeneratedArgs);
        }
    }

    public GeneratorExecutionContext GeneratorExecutionContext { get; }

    public IReadOnlyList<IAmAMGenExtension> Extensions { get; }

    public IReadOnlyList<Candidate> Candidates { get; }

    public IReadOnlyList<FileGenerator> Files => _files;
    readonly List<FileGenerator> _files = new();

    public IReadOnlyList<IHandleCodeGeneration> CodeGenerators => _codeGenerators;
    public void Add(IHandleCodeGeneration codeGenerator) => _codeGenerators.Add(codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator)));
    readonly List<IHandleCodeGeneration> _codeGenerators = new();
}