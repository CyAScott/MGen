using System.Diagnostics;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.Clone;

/// <summary>
/// Implements <see cref="System.ICloneable"/> for classes that require it.
/// </summary>
[DebuggerStepThrough, MGenExtension(Id, before: new [] { MemberDeclaration.Id })]
public class CloneSupport : IHandleOnInit, IHandleOnTypeCreated
{
    public const string Id = "MGen." + nameof(CloneSupport);

    public void Init(InitArgs args) => args.Context.Add(new CloneCodeGenerator());

    public void TypeCreated(TypeCreatedArgs args)
    {
        //todo: inject constructor
    }
}

[MGenExtension(Id, before: new [] { DefaultCodeGenerator.Id })]
public class CloneCodeGenerator : IHandleConstructorCodeGeneration, IHandleMethodCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(CloneCodeGenerator);

    public void Handle(ConstructorCodeGenerationArgs args)
    {
        //todo: create clone constructor
    }

    public void Handle(MethodCodeGenerationArgs args)
    {
        //todo: create clone method
    }
}