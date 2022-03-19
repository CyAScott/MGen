using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions;

/// <summary>
/// Adds required members to a class declaration based on the interfaces it needs to implement.
/// </summary>
[MGenExtension(Id), DebuggerStepThrough]
public partial class MemberDeclaration : IHandleOnInit, IHandleOnTypeCreated
{
    Dictionary<string, MemberGroupInfo> GetMembers(IHaveInheritance item)
    {
        var members = new Dictionary<string, MemberGroupInfo>();

        foreach (var code in item.Inheritance.OfType<CodeWithInheritedTypeSymbol>())
        {
            if (code.InheritedTypeSymbol is INamedTypeSymbol @interface)
            {
                Add(members, @interface);
            }
        }

        return members;
    }

    public const string Id = "MGen." + nameof(MemberDeclaration);

    public void Init(InitArgs args) => args.Context.Add(new DefaultCodeGenerator());

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveMembersWithCode builder and IHaveInheritance item)
        {
            var index = builder.Count;
            var members = GetMembers(item);

            foreach (var memberGroupInfo in members.Values)
            {
                memberGroupInfo.AddTo(builder);
            }

            for (; index < builder.Count; index++)
            {
                switch (builder[index])
                {
                    case MethodBuilder method:
                        args.GenerateCode(method);
                        break;
                    case PropertyBuilder property:
                        args.GenerateCode(property);
                        break;
                }
            }
        }
    }

    void Add(Dictionary<string, MemberGroupInfo> members, ISymbol member)
    {
        var name = member.Name;

        if (!name.StartsWith("get_") &&
            !name.StartsWith("set_") &&
            !name.StartsWith("add_") &&
            !name.StartsWith("remove_"))
        {
            if (!members.TryGetValue(name, out var memberGroupInfo))
            {
                members[name] = memberGroupInfo = new();
            }

            memberGroupInfo.TryToAdd(member);
        }
    }

    void Add(Dictionary<string, MemberGroupInfo> members, INamedTypeSymbol @interface)
    {
        foreach (var member in @interface.GetMembers())
        {
            Add(members, member);
        }

        foreach (var inheritedInterface in @interface.AllInterfaces)
        {
            Add(members, inheritedInterface);
        }
    }
}
