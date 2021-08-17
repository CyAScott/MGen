using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingMethods[] MethodBuilders { get; } = new IHandleBuildingMethods[]
        {
            WriteCloneSupport.Instance,
            WriteNetSerialization.Instance,
            WriteDefaultMethod.Instance
        };

        protected void AppendMethod(MethodBuilderContext context, int index = 0)
        {
            if (index < MethodBuilders.Length)
            {
                MethodBuilders[index].Handle(context, () => AppendMethod(context, index + 1));
            }
        }
    }
}
