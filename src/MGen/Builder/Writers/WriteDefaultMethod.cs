using MGen.Builder.BuilderContext;
using System;
using System.Linq;

namespace MGen.Builder.Writers
{
    class WriteDefaultMethod : IHandleBuildingMethods
    {
        public static readonly WriteDefaultMethod Instance = new();

        public void Handle(MethodBuilderContext context, Action next)
        {
            if (!context.Explicit)
            {
                if (context.Modifiers.Any(it => it.ValueText == "partial"))
                {
                    return;
                }

                context.Builder
                    .AppendXmlComments(context.Member)
                    .AppendAttributes(context.Member)
                    .Append("public ");
            }

            context.Builder.Append(context.Method.ReturnType).String.Append(' ');

            if (context.Explicit)
            {
                context.Builder.Append(context.Method.OriginalDefinition.ContainingSymbol).String.Append('.');
            }

            context.Builder.String.Append(context.Method.Name);

            context.Builder
                .AppendGenericNames(context.Method.TypeArguments)
                .Append("(").AppendParameters(context.Method.Parameters).AppendLine(")")
                .AppendGenericConstraints(context.Method.TypeArguments)
                .OpenBrace()
                .AppendLine("throw new System.NotImplementedException();")
                .CloseBrace()
                .AppendLine();
        }
    }
}
