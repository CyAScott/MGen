using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnInit : IAmAMGenExtension
{
    void Init(InitArgs args);
}

[DebuggerStepThrough]
public class InitArgs
{
    public InitArgs(GeneratorContext context) => Context = context;

    public GeneratorContext Context { get; }
}