using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleMethodCodeGeneration : IHandleCodeGeneration
{
    void Handle(MethodCodeGenerationArgs args);
}

[DebuggerStepThrough]
public class MethodCodeGenerationArgs
{
    public MethodCodeGenerationArgs(GeneratorContext context, MethodBuilder builder)
    {
        Builder = builder;
        Context = context;
    }

    public GeneratorContext Context { get; }

    public MethodBuilder Builder { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandleMethodCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class HandlerCollection
{
    readonly IReadOnlyList<IHandleMethodCodeGeneration>? _methodCodeGenerators;

    internal void GenerateCode(MethodBuilder builder)
    {
        if (_context != null && _methodCodeGenerators != null)
        {
            var args = new MethodCodeGenerationArgs(_context, builder);

            foreach (var generator in _methodCodeGenerators)
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