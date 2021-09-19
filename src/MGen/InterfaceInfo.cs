using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace MGen
{
    /// <summary>
    /// Represents an interface with it's MGen attributes and members.
    /// </summary>
    [DebuggerDisplay("{Type.ContainingNamespace.Name}.{Type.Name}")]
    class InterfaceInfo : Dictionary<string, InterfaceMemberInfo>
    {
        protected ITypeSymbol GetReturnType(ISymbol member)
        {
            if (member is IPropertySymbol propertySymbol)
            {
                return propertySymbol.Type;
            }

            if (member is IMethodSymbol methodSymbol)
            {
                return methodSymbol.ReturnType;
            }

            if (member is IEventSymbol eventSymbol)
            {
                return eventSymbol.Type;
            }

            throw new InvalidCastException();
        }

        /// <summary>
        /// Compares two method signatures to see if they are equal.
        /// </summary>
        protected bool ParametersEqual(ImmutableArray<IParameterSymbol> paramsA, ImmutableArray<IParameterSymbol> paramsB)
        {
            if (paramsA.Length != paramsB.Length)
            {
                return false;
            }

            for (var index = 0; index < paramsA.Length; index++)
            {
                var a = paramsA[index];
                var b = paramsB[index];

                if (!SymbolEqualityComparer.Default.Equals(a.Type, b.Type))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Recursively scans the interface for members.
        /// Members will be de-duplicated based on return types and signatures.
        /// </summary>
        protected void Add(ITypeSymbol @interface)
        {
            foreach (var member in @interface.GetMembers())
            {
                var name = member.Name;

                //ignore the auto generated methods for the properties and events
                if (name.StartsWith("get_") || name.StartsWith("set_") ||
                    name.StartsWith("add_") || name.StartsWith("remove_"))
                {
                    continue;
                }

                var returnType = GetReturnType(member);

                if (!TryGetValue(name, out var memberInfo))
                {
                    this[name] = new InterfaceMemberInfo(returnType, member);
                    continue;
                }

                //if the duplicate member has a different return type OR
                //is a different kind of member (i.e. property vs method) then every other member of the same name
                if (!SymbolEqualityComparer.Default.Equals(returnType, memberInfo.ReturnType) ||
                    memberInfo.Symbols.All(it => it.Kind != member.Kind))
                {
                    memberInfo.Symbols.Add(member);
                    continue;
                }

                //compare members that have parameters to de-duplicate members

                if (member is IPropertySymbol propertySymbol)
                {
                    if (!memberInfo.Symbols
                        .OfType<IPropertySymbol>()
                        .Any(it =>
                            it.IsIndexer == propertySymbol.IsIndexer &&
                            it.GetMethod == null == (propertySymbol.GetMethod == null) &&
                            it.SetMethod == null == (propertySymbol.SetMethod == null) &&
                            ParametersEqual(it.Parameters, propertySymbol.Parameters)))
                    {
                        memberInfo.Symbols.Add(member);
                    }
                }
                else if (member is IMethodSymbol methodSymbol)
                {
                    if (!memberInfo.Symbols
                        .OfType<IMethodSymbol>()
                        .Any(it => ParametersEqual(it.Parameters, methodSymbol.Parameters)))
                    {
                        memberInfo.Symbols.Add(member);
                    }
                }
            }

            foreach (var baseInterface in @interface.AllInterfaces)
            {
                Add(baseInterface);
            }
        }

        public InterfaceInfo(
            string path,
            ITypeSymbol type,
            SyntaxTokenList modifiers,
            List<MGenAttribute> attributes)
        {
            Attributes = attributes;
            Type = type;
            Modifiers = modifiers;
            Path = path.Split('.');

            Add(type);
        }

        public List<MGenAttribute> Attributes { get; }
        public ITypeSymbol Type { get; }
        public SyntaxTokenList Modifiers { get; }

        /// <summary>
        /// The path for the full name of the interface.
        /// This should contain the list of namespaces and class names where this interface is located in the code.
        /// </summary>
        public string[] Path { get; }
    }
}
