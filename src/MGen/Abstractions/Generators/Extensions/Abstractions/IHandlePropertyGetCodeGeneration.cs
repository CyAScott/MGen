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
    public PropertyGetCodeGenerationArgs(GeneratorContext context, PropertyBuilder builder)
    {
        Builder = builder;
        Context = context;
    }

    public GeneratorContext Context { get; }

    public PropertyBuilder Builder { get; }

    /// <summary>
    /// If true then additional invocations of <see cref="IHandlePropertyGetCodeGeneration"/> will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

partial class CodeGenerators
{
    readonly IReadOnlyList<IHandlePropertyGetCodeGeneration>? _propertyGetCodeGenerators;

    void GeneratePropertyGetCode(PropertyBuilder builder)
    {
        if (_context != null && _propertyGetCodeGenerators != null && builder.Get.Enabled)
        {
            var args = new PropertyGetCodeGenerationArgs(_context, builder);

            foreach (var generator in _propertyGetCodeGenerators)
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