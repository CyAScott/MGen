using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        IHandleBuildingEvents[] BuildEventBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingEvents>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingEvents handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WriteDefaultEvent.Instance);
            return list.ToArray();
        }

        protected IHandleBuildingEvents[] EventBuilders { get; }

        protected void AppendEvent(EventBuilderContext context, int index = 0)
        {
            if (index < EventBuilders.Length)
            {
                EventBuilders[index].Handle(context, () => AppendEvent(context, index + 1));
            }
        }
    }
}
