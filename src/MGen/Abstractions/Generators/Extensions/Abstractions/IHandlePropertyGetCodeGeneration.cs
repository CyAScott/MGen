using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandlePropertyGetCodeGeneration : IHandleCodeGeneration
{
    void Handle(PropertyGetCodeGenerationArgs args);
}

[DebuggerStepThrough]
public class PropertyGetCodeGenerationArgs
{
    public PropertyGetCodeGenerationArgs(GeneratorContext context, TypeGenerator generator, PropertyBuilder builder)
    {
        Builder = builder;
        Context = context;
        Generator = generator;
    }

    public GeneratorContext Context { get; }

    public PropertyBuilder Builder { get; }

    public TypeGenerator Generator { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandlePropertyGetCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class TypeGeneratedArgs
{
    public IReadOnlyList<IHandlePropertyGetCodeGeneration> PropertyGetCodeGenerators { get; }

    void GeneratePropertyGetCode(PropertyBuilder builder)
    {
        if (builder.Get.Enabled)
        {
            var args = new PropertyGetCodeGenerationArgs(Context, Generator, builder);

            foreach (var generator in PropertyGetCodeGenerators)
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