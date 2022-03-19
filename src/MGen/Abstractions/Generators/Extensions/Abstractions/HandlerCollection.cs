using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

[DebuggerStepThrough]
public partial class HandlerCollection
{
    readonly GeneratorContext? _context;
    readonly IReadOnlyList<IHandleOnTypeCreated>? _typeCreatedHandlers;

    internal HandlerCollection()
    {
        _constructorCodeGenerators = null;
        _context = null;
        _methodCodeGenerators = null;
        _propertyGetCodeGenerators = null;
        _propertySetCodeGenerators = null;
        _typeCreatedHandlers = null;
    }

    internal HandlerCollection(GeneratorContext context,
        IReadOnlyList<IHandleCodeGeneration> codeGenerators,
        IReadOnlyList<IHandleOnTypeCreated> typeCreatedHandlers)
    {
        var constructorCodeGeneratorCollection = new ExtensionsCollection<IHandleConstructorCodeGeneration>();
        var methodCodeGeneratorCollection = new ExtensionsCollection<IHandleMethodCodeGeneration>();
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
        _propertyGetCodeGenerators = propertyGetCodeGeneratorCollection.ToSortedList();
        _propertySetCodeGenerators = propertySetCodeGeneratorCollection.ToSortedList();
        _typeCreatedHandlers = typeCreatedHandlers;
    }

    internal void GenerateCode(PropertyBuilder builder)
    {
        GeneratePropertyGetCode(builder);
        GeneratePropertySetCode(builder);
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
}