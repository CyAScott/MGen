using MGen.Builder.BuilderContext;
using System;
using System.Linq;

namespace MGen.Builder.Writers
{
    partial class WriteDefaultMethod : IHandleBuildingMethods
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

            //todo: append generic args

            context.Builder.Append("(").AppendParameters(context.Method.Parameters).AppendLine(")");

            //todo: append generic constraints

            context.Builder.OpenBrace();
            context.Builder.AppendLine("throw new System.NotImplementedException();");
            context.Builder.CloseBrace();
            context.Builder.AppendLine();
        }
    }
}
