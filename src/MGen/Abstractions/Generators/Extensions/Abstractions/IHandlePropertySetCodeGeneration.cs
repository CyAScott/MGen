using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandlePropertySetCodeGeneration : IHandleCodeGeneration
{
    void Handle(PropertySetCodeGenerationArgs args);
}

[DebuggerStepThrough]
public class PropertySetCodeGenerationArgs
{
    public PropertySetCodeGenerationArgs(GeneratorContext context, TypeGenerator generator, PropertyBuilder builder)
    {
        Builder = builder;
        Context = context;
        Generator = generator;
    }

    public GeneratorContext Context { get; }

    public PropertyBuilder Builder { get; }

    public TypeGenerator Generator { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandlePropertySetCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class TypeGeneratedArgs
{
    public IReadOnlyList<IHandlePropertySetCodeGeneration> PropertySetCodeGenerators { get; }

    void GeneratePropertySetCode(PropertyBuilder builder)
    {
        if (builder.Set.Enabled)
        {
            var args = new PropertySetCodeGenerationArgs(Context, Generator, builder);

            foreach (var generator in PropertySetCodeGenerators)
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