using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

partial class MemberDeclaration
{
    [DebuggerStepThrough]
    class MemberInfo
    {
        public MemberInfo(ISymbol member)
        {
            Member = member;
            
            switch (member)
            {
                case IPropertySymbol propertySymbol:
                    Parameters = propertySymbol.IsIndexer ? propertySymbol.Parameters : ImmutableArray<IParameterSymbol>.Empty;
                    ReturnType = propertySymbol.Type;
                    return;
                case IMethodSymbol methodSymbol:
                    Parameters = methodSymbol.Parameters;
                    ReturnType = methodSymbol.ReturnType;
                    return;
                case IEventSymbol eventSymbol:
                    Parameters = ImmutableArray<IParameterSymbol>.Empty;
                    ReturnType = eventSymbol.Type;
                    return;
            }

            throw new InvalidCastException();
        }

        public ImmutableArray<IParameterSymbol> Parameters { get; }

        public ITypeSymbol ReturnType { get; }

        public ISymbol Member { get; }

        public bool ParametersEqual(ImmutableArray<IParameterSymbol> parameters)
        {
            if (Parameters.Length != parameters.Length)
            {
                return false;
            }

            for (var index = 0; index < Parameters.Length; index++)
            {
                var a = Parameters[index];
                var b = parameters[index];

                if (!SymbolEqualityComparer.Default.Equals(a.Type, b.Type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}