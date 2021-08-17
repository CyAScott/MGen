using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MGen
{
    class InterfaceMemberInfo
    {
        public InterfaceMemberInfo(ITypeSymbol returnType, ISymbol member)
        {
            Kind = member.Kind;
            Name = member.Name;
            ReturnType = returnType;
            Symbols = new List<ISymbol>
            {
                member
            };
        }

        public IList<ISymbol> Symbols { get; }

        public int IndexOfSecondaryProperty()
        {
            if (Symbols.Count < 2 ||
                Symbols[0] is not IPropertySymbol primaryProperty ||
                primaryProperty.GetMethod != null && primaryProperty.SetMethod != null)
            {
                return -1;
            }

            for (var index = 1; index < Symbols.Count; index++)
            {
                if (Symbols[index] is IPropertySymbol secondaryProperty &&
                    SymbolEqualityComparer.Default.Equals(primaryProperty.Type, secondaryProperty.Type))
                {
                    return index;
                }
            }

            return -1;
        }

        public ITypeSymbol ReturnType { get; }

        public SymbolKind Kind { get; }

        public string Name { get; }
    }
}
