using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnTypeCreated : IAmAMGenExtension
{
    void TypeCreated(TypeCreatedArgs args);
}

[DebuggerStepThrough]
public partial class TypeCreatedArgs
{
    public TypeCreatedArgs(GeneratorContext context, FileGenerator generator,
        IHaveMembers builder,
        IReadOnlyList<IHandleConstructorCodeGeneration> constructorCodeGenerators,
        IReadOnlyList<IHandleMethodCodeGeneration> methodCodeGenerators,
        IReadOnlyList<IHandlePropertyGetCodeGeneration> propertyGetCodeGenerators,
        IReadOnlyList<IHandlePropertySetCodeGeneration> propertySetCodeGenerators)
    {
        Builder = builder;
        ConstructorCodeGenerators = constructorCodeGenerators;
        Context = context;
        Generator = generator;
        MethodCodeGenerators = methodCodeGenerators;
        PropertyGetCodeGenerators = propertyGetCodeGenerators;
        PropertySetCodeGenerators = propertySetCodeGenerators;
    }

    public GeneratorContext Context { get; }

    public FileGenerator Generator { get; }

    public IHaveMembers Builder { get; }

    public void GenerateCode(PropertyBuilder builder)
    {
        GeneratePropertyGetCode(builder);
        GeneratePropertySetCode(builder);
    }
}