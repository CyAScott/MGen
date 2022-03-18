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

        CreateAndSortCodeGenerators(
            out var constructorCodeGenerators,
            out var methodCodeGenerators,
            out var propertyGetCodeGenerators,
            out var propertySetCodeGenerators);

        foreach (var candidate in Candidates)
        {
            if (TypeGenerator.TryToCreate(this, candidate, out var generator))
            {
                _typeGenerators.Add(generator);

                var typeGeneratedArgs = new TypeGeneratedArgs(this, generator,
                    constructorCodeGenerators,
                    methodCodeGenerators,
                    propertyGetCodeGenerators,
                    propertySetCodeGenerators);
                foreach (var extension in Extensions.OfType<IHandleOnTypeGenerated>())
                {
                    extension.TypeGenerated(typeGeneratedArgs);
                }
            }
        }

        var typesGeneratedArgs = new TypesGeneratedArgs(this);
        foreach (var extension in Extensions.OfType<IHandleOnTypesGenerated>())
        {
            extension.TypesGenerated(typesGeneratedArgs);
        }

        var builder = new StringBuilder();

        foreach (var generator in _typeGenerators)
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

    public IReadOnlyList<TypeGenerator> TypeGenerators => _typeGenerators;
    readonly List<TypeGenerator> _typeGenerators = new();

    public IReadOnlyList<IHandleCodeGeneration> CodeGenerators => _codeGenerators;
    public void Add(IHandleCodeGeneration codeGenerator) => _codeGenerators.Add(codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator)));
    readonly List<IHandleCodeGeneration> _codeGenerators = new();
    void CreateAndSortCodeGenerators(
        out List<IHandleConstructorCodeGeneration> constructorCodeGenerators,
        out List<IHandleMethodCodeGeneration> methodCodeGenerators,
        out List<IHandlePropertyGetCodeGeneration> propertyGetCodeGenerators,
        out List<IHandlePropertySetCodeGeneration> propertySetCodeGenerators)
    {
        var constructorCodeGeneratorCollection = new ExtensionsCollection<IHandleConstructorCodeGeneration>();
        var methodCodeGeneratorCollection = new ExtensionsCollection<IHandleMethodCodeGeneration>();
        var propertyGetCodeGeneratorCollection = new ExtensionsCollection<IHandlePropertyGetCodeGeneration>();
        var propertySetCodeGeneratorCollection = new ExtensionsCollection<IHandlePropertySetCodeGeneration>();

        foreach (var codeGenerator in CodeGenerators)
        {
            if (codeGenerator is IHandleConstructorCodeGeneration constructorCodeGenerator)
            {
                constructorCodeGeneratorCollection.Add(constructorCodeGenerator);
            }

            if (codeGenerator is IHandleMethodCodeGeneration methodCodeGenerator)
            {
                methodCodeGeneratorCollection.Add(methodCodeGenerator);
            }

            if (codeGenerator is IHandlePropertyGetCodeGeneration propertyGetCodeGenerator)
            {
                propertyGetCodeGeneratorCollection.Add(propertyGetCodeGenerator);
            }

            if (codeGenerator is IHandlePropertySetCodeGeneration propertySetCodeGenerator)
            {
                propertySetCodeGeneratorCollection.Add(propertySetCodeGenerator);
            }
        }

        constructorCodeGenerators = constructorCodeGeneratorCollection.ToSortedList();
        methodCodeGenerators = methodCodeGeneratorCollection.ToSortedList();
        propertyGetCodeGenerators = propertyGetCodeGeneratorCollection.ToSortedList();
        propertySetCodeGenerators = propertySetCodeGeneratorCollection.ToSortedList();
    }
}