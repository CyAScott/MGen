using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;

namespace MGen.Builder.Writers
{
    partial class WriteDefaultClass 
    {
        public static readonly WriteDefaultClass Instance = new();

        public void Write(ClassBuilderContext context, Action next)
        {
            context.Builder.AppendXmlComments(context.Interface);

            var builder = context.Builder.AppendIndent().String;
            foreach (var modifier in context.Modifiers)
            {
                builder.Append(modifier).Append(' ');
            }
            builder.Append("class ").Append(context.ClassName);

            var typeArguments = (context.Interface as INamedTypeSymbol)?.TypeArguments;

            if (typeArguments != null && typeArguments.Value.Length > 0)
            {
                Write(context, typeArguments.Value);
            }

            builder.Append(" : ").Append(context.Interface.Name);

            if (typeArguments != null && typeArguments.Value.Length > 0)
            {
                Write(context, typeArguments.Value);

                //todo: write generic constraints
            }

            context.Builder.AppendLine();

            context.Builder.OpenBrace();

            next();

            context.Builder.CloseBrace();
        }

        public void Write(ClassBuilderContext context, ImmutableArray<ITypeSymbol> genericArguments)
        {
            var builder = context.Builder.String;

            builder.Append('<');

            for (var index = 0; index < genericArguments.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(genericArguments[index]);
            }

            builder.Append('>');
        }
    }

    partial class WriteDefaultClass : IHandleBuildingClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);
    }

    partial class WriteDefaultClass : IHandleBuildingNestedClasses
    {
        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);
    }
}
