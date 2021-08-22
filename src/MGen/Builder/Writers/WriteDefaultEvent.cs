using MGen.Builder.BuilderContext;
using System;

namespace MGen.Builder.Writers
{
    partial class WriteDefaultEvent : IHandleBuildingEvents
    {
        public static readonly WriteDefaultEvent Instance = new();

        public void Handle(EventBuilderContext context, Action next)
        {
            if (!context.Explicit)
            {
                context.Builder
                    .AppendXmlComments(context.Member)
                    .AppendAttributes(context.Member)
                    .Append("public ");
            }

            context.Builder.Append("event ").String.AppendType(context.Event.Type).Append(' ');

            if (context.Explicit)
            {
                context.Builder.Append(context.Event.OriginalDefinition.ContainingSymbol).String.Append('.');
            }

            context.Builder.String.Append(context.Event.Name);

            context.Builder.AppendLine(";").AppendLine();
        }
    }
}
