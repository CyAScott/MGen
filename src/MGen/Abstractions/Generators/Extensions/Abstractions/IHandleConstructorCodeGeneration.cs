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
    public ConstructorCodeGenerationArgs(GeneratorContext context, FileGenerator generator, ConstructorBuilder builder)
    {
        Builder = builder;
        Context = context;
        Generator = generator;
    }

    public ConstructorBuilder Builder { get; }

    public GeneratorContext Context { get; }

    public FileGenerator Generator { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandleConstructorCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class TypeCreatedArgs
{
    public IReadOnlyList<IHandleConstructorCodeGeneration> ConstructorCodeGenerators { get; }

    public void GenerateCode(ConstructorBuilder builder)
    {
        var args = new ConstructorCodeGenerationArgs(Context, Generator, builder);

        foreach (var generator in ConstructorCodeGenerators)
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