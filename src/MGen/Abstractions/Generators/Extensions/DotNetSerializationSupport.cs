using System.Diagnostics;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions;

/// <summary>
/// Implements <see cref="System.Runtime.Serialization.ISerializable"/> for classes that require it.
/// </summary>
[DebuggerStepThrough, MGenExtension(Id, after: new[] { MembersWithCodeDeclaration.Id }, before: new [] { MemberDeclaration.Id })]
public class DotNetSerializationSupport : IHandleOnInit, IHandleOnTypeGenerated
{
    public const string Id = "MGen." + nameof(DotNetSerializationSupport);

    public void Init(InitArgs args) => args.Context.Add(new DotNetSerializationCodeGenerator());

    public void TypeGenerated(TypeGeneratedArgs args)
    {
        //todo: inject constructor
    }
}

[MGenExtension(Id, before: new [] { DefaultCodeGenerator.Id })]
public class DotNetSerializationCodeGenerator : IHandleConstructorCodeGeneration, IHandleMethodCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(DotNetSerializationCodeGenerator);

    public void Handle(ConstructorCodeGenerationArgs args)
    {
        //todo: create protected constructor
    }

    public void Handle(MethodCodeGenerationArgs args)
    {
        //todo: create GetObjectData method
    }
}