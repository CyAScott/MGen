using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

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

            context.Builder.AppendGenericNames(typeArguments);

            builder.Append(" : ").Append(context.Interface.Name);

            context.Builder
                .AppendGenericNames(typeArguments).AppendLine()
                .AppendGenericConstraints(typeArguments)
                .OpenBrace();

            next();

            context.Builder.CloseBrace();
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
