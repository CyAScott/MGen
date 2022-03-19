using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleConstructorCodeGeneration : IHandleCodeGeneration
{
    void Handle(ConstructorCodeGenerationArgs args);
}

[DebuggerStepThrough]
public class ConstructorCodeGenerationArgs
{
    public ConstructorCodeGenerationArgs(GeneratorContext context, ConstructorBuilder builder)
    {
        Builder = builder;
        Context = context;
    }

    public ConstructorBuilder Builder { get; }

    public GeneratorContext Context { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandleConstructorCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class HandlerCollection
{
    readonly IReadOnlyList<IHandleConstructorCodeGeneration>? _constructorCodeGenerators;

    internal void GenerateCode(ConstructorBuilder builder)
    {
        if (_context != null && _constructorCodeGenerators != null)
        {
            var args = new ConstructorCodeGenerationArgs(_context, builder);

            foreach (var generator in _constructorCodeGenerators)
            {
                if (generator.Enabled)
                {
                    generator.Handle(args);
                    if (args.Handled)
                    {
                        break;
                    }
                }
            }
        }
    }
}