using System.Diagnostics;
using MGen.Abstractions.Builders;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnTypeCreated : IAmAMGenExtension
{
    void TypeCreated(TypeCreatedArgs args);
}

[DebuggerStepThrough]
public class TypeCreatedArgs
{
    public TypeCreatedArgs(GeneratorContext context,
        IHaveTypes container, IHaveMembers builder)
    {
        Builder = builder;
        Container = container;
        Context = context;
    }

    public GeneratorContext Context { get; }

    public IHaveTypes Container { get; }

    public IHaveMembers Builder { get; }
}