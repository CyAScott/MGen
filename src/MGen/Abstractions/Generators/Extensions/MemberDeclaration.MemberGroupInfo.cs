using System;
using System.Collections.Generic;
using System.Diagnostics;
using MGen.Abstractions.Builders.Members;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions;

partial class MemberDeclaration
{
    [DebuggerStepThrough]
    class MemberGroupInfo : List<MemberInfo>
    {
        bool _get, _set;

        int IndexOf(int start, Func<MemberInfo, bool> predicate)
        {
            for (; start < Count; start++)
            {
                if (predicate(this[start]))
                {
                    return start;
                }
            }

            return -1;
        }

        public void TryToAdd(ISymbol member)
        {
            var newMemberInfo = new MemberInfo(member);

            //detect if this is a duplicate member
            foreach (var memberInfo in this)
            {
                if (newMemberInfo.Member.Kind == memberInfo.Member.Kind &&
                    SymbolEqualityComparer.Default.Equals(newMemberInfo.ReturnType, memberInfo.ReturnType) &&
                    newMemberInfo.ParametersEqual(memberInfo.Parameters) &&
                    //a property set and set could be declared separately
                    (this[0].Member.Kind != SymbolKind.Property || _get && _set))
                {
                    return;
                }
            }

            Add(newMemberInfo);

            if (this[0].Member.Kind == SymbolKind.Property && member is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.GetMethod != null)
                {
                    _get = true;
                }

                if (propertySymbol.SetMethod != null)
                {
                    _set = true;
                }
            }
        }

        public void AddTo(IHaveMembersWithCode builder)
        {
            var first = this[0];
            var skipIndices = new HashSet<int>();

            for (var index = 0; index < Count; index++)
            {
                if (skipIndices.Contains(index))
                {
                    continue;
                }

                ICanHaveAnExplicitDeclaration declaration;
                var memberInfo = this[index];

                switch (memberInfo.Member)
                {
                    case IPropertySymbol propertySymbol:
                        if (index > 0 || propertySymbol.GetMethod != null && propertySymbol.SetMethod != null)
                        {
                            declaration = builder.AddProperty(propertySymbol);
                        }
                        else if (propertySymbol.GetMethod != null)
                        {
                            var setIndex = IndexOf(index + 1, next => next.Member is IPropertySymbol { SetMethod: { } });
                            declaration = setIndex == -1 ? builder.AddProperty(propertySymbol) : builder.AddProperty(propertySymbol, (IPropertySymbol)this[setIndex].Member);
                            skipIndices.Add(setIndex);
                        }
                        else
                        {
                            var getIndex = IndexOf(index + 1, next => next.Member is IPropertySymbol { SetMethod: { } });
                            declaration = getIndex == -1 ? builder.AddProperty(propertySymbol) : builder.AddProperty(propertySymbol, (IPropertySymbol)this[getIndex].Member);
                            skipIndices.Add(getIndex);
                        }
                        break;
                    case IMethodSymbol methodSymbol:
                        declaration = builder.AddMethod(methodSymbol);
                        break;
                    case IEventSymbol eventSymbol:
                        declaration = builder.AddEvent(eventSymbol);
                        break;
                    default:
                        continue;
                }

                declaration.ExplicitDeclaration.IsExplicitDeclarationEnabled =
                    index > 0 &&
                    (
                        first.Parameters.Length == 0 ||
                        first.Member.Kind != memberInfo.Member.Kind ||
                        first.ParametersEqual(memberInfo.Parameters)
                    );
            }
        }
    }
}