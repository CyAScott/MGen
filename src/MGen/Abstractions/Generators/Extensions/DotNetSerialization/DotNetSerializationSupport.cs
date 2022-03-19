using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions.DotNetSerialization;

/// <summary>
/// Implements <see cref="ISerializable"/> for classes that require it.
/// </summary>
[MGenExtension(Id, before: new [] { MemberDeclaration.Id }), DebuggerStepThrough]
public class DotNetSerializationSupport : IHandleOnInit, IHandleOnTypeCreated
{
    INamedTypeSymbol? _attribute;

    public const string Id = "MGen." + nameof(DotNetSerializationSupport);

    public const string InterfaceName = nameof(ISerializable);

    public void Init(InitArgs args) => args.Context.Add(new DotNetSerializationCodeGenerator());

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveAttributes attributes and IHaveConstructors builder and IHaveMembersWithCode and IHaveInheritance item)
        {
            foreach (var code in item.Inheritance.OfType<CodeWithInheritedTypeSymbol>())
            {
                if (code.InheritedTypeSymbol.IsSerializable())
                {
                    _attribute ??= args.Context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.SerializableAttribute");

                    if (_attribute != null &&
                        !attributes.Attributes
                            .Select(it => it.Type)
                            .OfType<CodeType>()
                            .Select(it => it.Type)
                            .Any(it => SymbolEqualityComparer.Default.Equals(it, _attribute)))
                    {
                        attributes.Attributes.Add(_attribute);
                    }

                    var ctor = builder.AddConstructor();

                    ctor.ArgumentParameters.Add("System.Runtime.Serialization.SerializationInfo", "info")
                        .Attributes.Add("System.Diagnostics.CodeAnalysis.NotNullAttribute");
                    ctor.ArgumentParameters.Add("System.Runtime.Serialization.StreamingContext", "context")
                        .Attributes.Add("System.Diagnostics.CodeAnalysis.NotNullAttribute");
                    ctor.Modifiers.IsProtected = true;
                    ctor.State[InterfaceName] = true;
                    ctor.XmlComments.Add("Deserializes the class.");

                    return;
                }
            }
        }
    }
}

/// <summary>
/// Implements <see cref="ISerializable"/> for classes that require it.
/// </summary>
[MGenExtension(Id, after: new[] { MemberDeclaration.Id }), DebuggerStepThrough]
public class DotNetSerializationConstructorCodeGenerator : IHandleOnTypeCreated
{
    public const string Id = "MGen." + nameof(DotNetSerializationConstructorCodeGenerator);

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveConstructors builder)
        {
            foreach (var ctor in builder.OfType<ConstructorBuilder>())
            {
                if (ctor.State.ContainsKey(DotNetSerializationSupport.InterfaceName))
                {
                    ctor.GenerateCode();
                }
            }
        }
    }
}

[MGenExtension(Id, before: new [] { DefaultCodeGenerator.Id }), DebuggerStepThrough]
public partial class DotNetSerializationCodeGenerator
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(DotNetSerializationCodeGenerator);
}
