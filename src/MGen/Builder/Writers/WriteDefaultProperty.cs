using MGen.Builder.BuilderContext;
using System;
using System.Linq;

namespace MGen.Builder.Writers
{
    class WriteDefaultProperty : IHandleBuildingProperties
    {
        public static WriteDefaultProperty Instance = new();

        public void Handle(PropertyBuilderContext context, Action next)
        {
            if (!context.Explicit)
            {
                context.Builder
                    .AppendXmlComments(context.Member)
                    .Append("public ");
            }
            else if (context.Modifiers.Any(it => it.ValueText == "partial"))
            {
                return;
            }

            context.Builder.Append(context.Primary.Type).String.Append(' ');

            if (context.Explicit)
            {
                context.Builder.Append(context.Primary.OriginalDefinition.ContainingSymbol).String.Append('.');
            }

            if (context.Primary.IsIndexer)
            {
                WriteIndexer(context);
            }
            else
            {
                context.Builder.String.Append(context.Primary.Name);
            }
            
            if (context.Explicit)
            {
                WriteExplicitGetSet(context);
            }
            else
            {
                WriteGetSet(context, next);
            }

            context.Builder.AppendLine("");
        }

        public void WriteExplicitGetSet(PropertyBuilderContext context)
        {
            context.Builder.String.Append(" { ");

            if (context.HasGet)
            {
                context.Builder.String.Append("get; ");
            }

            if (context.HasSet)
            {
                context.Builder.AppendLine("set; }");
            }
            else
            {
                context.Builder.AppendLine("}");
            }
        }

        public void WriteGetSet(PropertyBuilderContext context, Action next)
        {
            context.Builder.AppendLine();
            context.Builder.OpenBrace();

            next();

            context.Builder.CloseBrace();

            if (context.FieldName != null)
            {
                context.Builder.AppendLine(builder => builder.Append("private ").AppendType(context.Primary.Type).Append(' ').Append(context.FieldName).Append(';'));
            }
        }

        public void WriteIndexer(PropertyBuilderContext context)
        {
            context.Builder.String.Append("this[");
            context.Builder.AppendParameters(context.Primary.Parameters);
            context.Builder.String.Append(']');
        }
    }
}
