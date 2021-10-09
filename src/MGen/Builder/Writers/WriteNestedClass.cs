using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Builder.Writers
{
    partial class WriteNestedClass : IHandleBuildingProperties
    {
        public static readonly WriteNestedClass Instance = new();

        public ITypeSymbol? GetNestedClass(PropertyBuilderContext context, ITypeSymbol type)
        {
            if (type is IArrayTypeSymbol arrayType)
            {
                return GetNestedClass(context, arrayType.ElementType);
            }

            if (IsGenericCollection(context, type, out var elementType))
            {
                return GetNestedClass(context, elementType);
            }

            if (type.TypeKind == TypeKind.Interface)
            {
                return type;
            }

            return null;
        }

        public bool IsGenericCollection(PropertyBuilderContext context, ITypeSymbol type, out ITypeSymbol elementType)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8601 // Possible null reference assignment.

            if (!context.CollectionGenerators.TryToGet(context, type, "test", out var generator))
            {
                elementType = null;
                return false;
            }

            elementType = generator.ValueType;

            return true;

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();

            if (context.Explicit)
            {
                return;
            }

            var nestedClassType = GetNestedClass(context, context.Primary.Type);
            if (nestedClassType != null)
            {
                context.Builder.Append(context, nestedClassType);
            }
        }
    }

    partial class WriteNestedClass : IHandleBuildingNestedClasses
    {
        public void Handle(NestedClassBuilderContext context, Action next)
        {
            throw new NotImplementedException();
        }
    }
}
