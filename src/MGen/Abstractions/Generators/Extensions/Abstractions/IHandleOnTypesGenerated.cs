using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnTypesGenerated : IAmAMGenExtension
{
    void TypesGenerated(TypesGeneratedArgs args);
}

[DebuggerStepThrough]
public class TypesGeneratedArgs
{
    public TypesGeneratedArgs(GeneratorContext context) => Context = context;

    public GeneratorContext Context { get; }
}