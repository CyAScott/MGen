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
    public MethodCodeGenerationArgs(GeneratorContext context, FileGenerator generator, MethodBuilder builder)
    {
        Builder = builder;
        Context = context;
        Generator = generator;
    }

    public GeneratorContext Context { get; }

    public MethodBuilder Builder { get; }

    public FileGenerator Generator { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandleMethodCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class TypeCreatedArgs
{
    public IReadOnlyList<IHandleMethodCodeGeneration> MethodCodeGenerators { get; }

    public void GenerateCode(MethodBuilder builder)
    {
        var args = new MethodCodeGenerationArgs(Context, Generator, builder);

        foreach (var generator in MethodCodeGenerators)
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