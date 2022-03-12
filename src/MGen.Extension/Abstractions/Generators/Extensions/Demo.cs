using System.Diagnostics;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions;

[DebuggerStepThrough, MGenExtension(Id, after: new[] { MemberDeclaration.Id })]
public class Demo : IHandleOnTypeGenerated
{
    public const string Id = "MGen." + nameof(Demo);

    public void TypeGenerated(TypeGeneratedArgs args)
    {
        if (args.Generator.State.TryGetValue(MembersWithCodeDeclaration.MembersWithCodeDeclarationKey, out var value) &&
            value is IHaveInheritance item and IHaveMembersWithCode builder)
        {
            var method = builder.AddMethod("int", "Get");

            method.Return(0);
        }
    }
}