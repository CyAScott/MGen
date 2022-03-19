using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.ReadOnlyConstructor;

/// <summary>
/// Creates a constructor for initializing read only members.
/// </summary>
[MGenExtension(Id, after: new [] { MemberDeclaration.Id }), DebuggerStepThrough]
public class ReadOnlyConstructorSupport : IHandleOnInit, IHandleOnTypeCreated
{
    public const string Id = "MGen." + nameof(ReadOnlyConstructorSupport);

    public void Init(InitArgs args) => args.Context.Add(new ReadOnlyConstructorGenerator());

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveMembersWithCode builder)
        {
            ConstructorBuilder? ctor = null;

            for (int count = builder.Count, index = 0; index < count; index++)
            {
                if (builder[index] is FieldBuilder { Enabled: true } field && field.Modifiers.IsReadonly)
                {
                    ctor ??= builder.AddConstructor();

                    ctor.ArgumentParameters
                        .Add(field.ReturnType, field.Name.Substring(1))
                        .State[nameof(ReadOnlyConstructorSupport)] = field;
                }
            }

            if (ctor != null)
            {
                ctor.Modifiers.IsPublic = true;
                ctor.State[nameof(ReadOnlyConstructorSupport)] = this;
                args.GenerateCode(ctor);
            }
        }
    }
}

[MGenExtension(Id)]
public class ReadOnlyConstructorGenerator : IHandleConstructorCodeGeneration
{
    [ExcludeFromCodeCoverage]
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(ReadOnlyConstructorGenerator);

    public void Handle(ConstructorCodeGenerationArgs args)
    {
        if (args.Builder.State.ContainsKey(nameof(ReadOnlyConstructorSupport)))
        {
            foreach (var pair in args.Builder.ArgumentParameters)
            {
                args.Builder.Set(
                    ((FieldBuilder)pair.Value.State[nameof(ReadOnlyConstructorSupport)]).Name,
                    pair.Key);
            }
        }
    }
}