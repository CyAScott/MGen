using System.Diagnostics;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions;

/// <summary>
/// Implements MGen.ISupportConversion for classes that require it.
/// </summary>
[DebuggerStepThrough, MGenExtension(Id, after: new[] { MemberDeclaration.Id })]
public class ConversionSupport : IHandleOnInit, IHandleOnTypeGenerated
{
    public const string Id = "MGen." + nameof(ConversionSupport);

    public void Init(InitArgs args) => args.Context.Add(new ConversionCodeGenerator());

    public void TypeGenerated(TypeGeneratedArgs args)
    {
        //todo: inject constructor
    }
}

[MGenExtension(Id, before: new[] { DefaultCodeGenerator.Id })]
public class ConversionCodeGenerator : IHandleConstructorCodeGeneration, IHandleMethodCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(ConversionCodeGenerator);

    public void Handle(ConstructorCodeGenerationArgs args)
    {
        //todo: create conversion constructor
    }

    public void Handle(MethodCodeGenerationArgs args)
    {
        //todo: create ISupportConversion methods
    }
}