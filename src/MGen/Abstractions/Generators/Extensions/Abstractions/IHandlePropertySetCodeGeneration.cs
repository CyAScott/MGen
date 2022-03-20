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
    public PropertySetCodeGenerationArgs(GeneratorContext context, PropertyBuilder builder)
    {
        Builder = builder;
        Context = context;
    }

    public GeneratorContext Context { get; }

    public PropertyBuilder Builder { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandlePropertySetCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class CodeGenerators
{
    readonly IReadOnlyList<IHandlePropertySetCodeGeneration>? _propertySetCodeGenerators;

    void GeneratePropertySetCode(PropertyBuilder builder)
    {
        if (_context != null && _propertySetCodeGenerators != null && builder.Set.Enabled)
        {
            var args = new PropertySetCodeGenerationArgs(_context, builder);

            foreach (var generator in _propertySetCodeGenerators)
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