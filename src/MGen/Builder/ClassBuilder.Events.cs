using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingEvents[] EventBuilders { get; } = new[]
        {
            WriteDefaultEvent.Instance
        };

        protected void AppendEvent(EventBuilderContext context, int index = 0)
        {
            if (index < EventBuilders.Length)
            {
                EventBuilders[index].Handle(context, () => AppendEvent(context, index + 1));
            }
        }
    }
}
