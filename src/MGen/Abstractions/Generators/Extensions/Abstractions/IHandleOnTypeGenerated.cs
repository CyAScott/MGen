using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnTypeGenerated : IAmAMGenExtension
{
    void TypeGenerated(TypeGeneratedArgs args);
}

[DebuggerStepThrough]
public partial class TypeGeneratedArgs
{
    public TypeGeneratedArgs(GeneratorContext context, TypeGenerator generator,
        IReadOnlyList<IHandleConstructorCodeGeneration> constructorCodeGenerators,
        IReadOnlyList<IHandleMethodCodeGeneration> methodCodeGenerators,
        IReadOnlyList<IHandlePropertyGetCodeGeneration> propertyGetCodeGenerators,
        IReadOnlyList<IHandlePropertySetCodeGeneration> propertySetCodeGenerators)
    {
        ConstructorCodeGenerators = constructorCodeGenerators;
        Context = context;
        Generator = generator;
        MethodCodeGenerators = methodCodeGenerators;
        PropertyGetCodeGenerators = propertyGetCodeGenerators;
        PropertySetCodeGenerators = propertySetCodeGenerators;
    }

    public GeneratorContext Context { get; }

    public TypeGenerator Generator { get; }

    public void GenerateCode(PropertyBuilder builder)
    {
        GeneratePropertyGetCode(builder);
        GeneratePropertySetCode(builder);
    }
}