using System.Collections.Generic;
using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnFileCreated : IAmAMGenExtension
{
    void FileCreated(FileCreatedArgs args);
}

[DebuggerStepThrough]
public class FileCreatedArgs
{
    readonly IReadOnlyList<IHandleConstructorCodeGeneration> _constructorCodeGenerators;
    readonly IReadOnlyList<IHandleMethodCodeGeneration> _methodCodeGenerators;
    readonly IReadOnlyList<IHandlePropertyGetCodeGeneration> _propertyGetCodeGenerators;
    readonly IReadOnlyList<IHandlePropertySetCodeGeneration> _propertySetCodeGenerators;

    public FileCreatedArgs(GeneratorContext context, FileGenerator generator,
        IReadOnlyList<IHandleConstructorCodeGeneration> constructorCodeGenerators,
        IReadOnlyList<IHandleMethodCodeGeneration> methodCodeGenerators,
        IReadOnlyList<IHandleOnTypeCreated> typeCreatedHandlers,
        IReadOnlyList<IHandlePropertyGetCodeGeneration> propertyGetCodeGenerators,
        IReadOnlyList<IHandlePropertySetCodeGeneration> propertySetCodeGenerators)
    {
        _constructorCodeGenerators = constructorCodeGenerators;
        _methodCodeGenerators = methodCodeGenerators;
        _propertyGetCodeGenerators = propertyGetCodeGenerators;
        _propertySetCodeGenerators = propertySetCodeGenerators;
        Context = context;
        Generator = generator;
        TypeCreatedHandlers = typeCreatedHandlers;
    }

    public GeneratorContext Context { get; }

    public FileGenerator Generator { get; }

    public IReadOnlyList<IHandleOnTypeCreated> TypeCreatedHandlers { get; }

    public void CreateType(IHaveMembers builder)
    {
        var args = new TypeCreatedArgs(Context, Generator, builder,
            _constructorCodeGenerators,
            _methodCodeGenerators,
            _propertyGetCodeGenerators,
            _propertySetCodeGenerators);

        foreach (var handler in TypeCreatedHandlers)
        {
            handler.TypeCreated(args);
        }
    }
}