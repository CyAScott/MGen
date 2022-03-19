using System.Diagnostics;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions;

[DebuggerStepThrough, MGenExtension(Id, after: new[] { MemberDeclaration.Id })]
public class Demo : IHandleOnTypeCreated
{
    public const string Id = "MGen." + nameof(Demo);

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveMembersWithCode builder)
        {
            var method = builder.AddMethod("int", "Get");

            method.Return(0);
        }
    }
}