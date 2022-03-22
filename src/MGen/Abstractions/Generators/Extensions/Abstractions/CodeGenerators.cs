﻿using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHaveCodeGenerators
{
    CodeGenerators CodeGenerators { get; }
}

public interface IInvokeCodeGenerators
{
    void GenerateCode();
}

[DebuggerStepThrough]
public partial class CodeGenerators
{
    readonly GeneratorContext? _context;
    readonly IReadOnlyList<IHandleOnTypeCreated>? _typeCreatedHandlers;

    internal CodeGenerators()
    {
        _constructorCodeGenerators = null;
        _context = null;
        _methodCodeGenerators = null;
        _propertyCodeGeneration = null;
        _propertyGetCodeGenerators = null;
        _propertySetCodeGenerators = null;
        _typeCreatedHandlers = null;
    }

    internal CodeGenerators(GeneratorContext context,
        IReadOnlyList<IHandleCodeGeneration> codeGenerators,
        IReadOnlyList<IHandleOnTypeCreated> typeCreatedHandlers)
    {
        var constructorCodeGeneratorCollection = new ExtensionsCollection<IHandleConstructorCodeGeneration>();
        var methodCodeGeneratorCollection = new ExtensionsCollection<IHandleMethodCodeGeneration>();
        var propertyCodeGeneratorCollection = new ExtensionsCollection<IHandlePropertyCodeGeneration>();
        var propertyGetCodeGeneratorCollection = new ExtensionsCollection<IHandlePropertyGetCodeGeneration>();
        var propertySetCodeGeneratorCollection = new ExtensionsCollection<IHandlePropertySetCodeGeneration>();

        foreach (var codeGenerator in codeGenerators)
        {
            if (codeGenerator is IHandleConstructorCodeGeneration constructorCodeGenerator)
            {
                constructorCodeGeneratorCollection.Add(constructorCodeGenerator);
            }

            if (codeGenerator is IHandleMethodCodeGeneration methodCodeGenerator)
            {
                methodCodeGeneratorCollection.Add(methodCodeGenerator);
            }

            if (codeGenerator is IHandlePropertyCodeGeneration propertyCodeGenerator)
            {
                propertyCodeGeneratorCollection.Add(propertyCodeGenerator);
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

        _constructorCodeGenerators = constructorCodeGeneratorCollection.ToSortedList();
        _context = context;
        _methodCodeGenerators = methodCodeGeneratorCollection.ToSortedList();
        _propertyCodeGeneration = propertyCodeGeneratorCollection.ToSortedList();
        _propertyGetCodeGenerators = propertyGetCodeGeneratorCollection.ToSortedList();
        _propertySetCodeGenerators = propertySetCodeGeneratorCollection.ToSortedList();
        _typeCreatedHandlers = typeCreatedHandlers;
    }

    internal void TypeCreated(IHaveTypes container, IHaveMembers type)
    {
        if (_context != null && _typeCreatedHandlers != null)
        {
            var args = new TypeCreatedArgs(_context, container, type);

            foreach (var handler in _typeCreatedHandlers)
            {
                handler.TypeCreated(args);
            }
        }
    }

    public Candidate? CurrentCandidate { get; internal set; }

    public FileGenerator? CurrentFile { get; internal set; }
}