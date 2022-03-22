using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandlePropertyCodeGeneration : IHandleCodeGeneration
{
    void Handle(PropertyCodeGenerationArgs args);
}

public interface IHandlePropertyGetCodeGeneration : IHandleCodeGeneration
{
    void Handle(PropertyGetCodeGenerationArgs args);
}

public interface IHandlePropertySetCodeGeneration : IHandleCodeGeneration
{
    void Handle(PropertySetCodeGenerationArgs args);
}

[DebuggerStepThrough]
public class PropertyCodeGenerationArgs
{
    public PropertyCodeGenerationArgs(GeneratorContext context, PropertyBuilder builder)
    {
        Builder = builder;
        Context = context;
    }

    public GeneratorContext Context { get; }

    public PropertyBuilder Builder { get; }

    /// <summary>
    /// If true then additional invocations of handler will be blocked.
    /// </summary>
    public bool Handled { get; set; }
}

[DebuggerStepThrough]
public class PropertyGetCodeGenerationArgs : PropertyCodeGenerationArgs
{
    public PropertyGetCodeGenerationArgs(GeneratorContext context, PropertyBuilder builder)
        : base(context, builder)
    {
    }
}

[DebuggerStepThrough]
public class PropertySetCodeGenerationArgs : PropertyCodeGenerationArgs
{
    public PropertySetCodeGenerationArgs(GeneratorContext context, PropertyBuilder builder)
        : base(context, builder)
    {
    }
}

partial class CodeGenerators
{
    readonly IReadOnlyList<IHandlePropertyCodeGeneration>? _propertyCodeGeneration;

    internal void GenerateCode(PropertyBuilder builder)
    {
        if (_context != null && _propertyCodeGeneration != null && builder.Enabled)
        {
            var args = new PropertyCodeGenerationArgs(_context, builder);

            foreach (var generator in _propertyCodeGeneration)
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

        GeneratePropertyGetCode(builder);
        GeneratePropertySetCode(builder);
    }

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