using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        IHandleBuildingMethods[] BuildMethodBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingMethods>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingMethods handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WriteCloneSupport.Instance);
            list.Add(WriteConversionSupport.Instance);
            list.Add(WriteNetSerialization.Instance);
            list.Add(WriteDefaultMethod.Instance);
            return list.ToArray();
        }

        protected IHandleBuildingMethods[] MethodBuilders { get; }

        protected void AppendMethod(MethodBuilderContext context, int index = 0)
        {
            if (index < MethodBuilders.Length)
            {
                MethodBuilders[index].Handle(context, () => AppendMethod(context, index + 1));
            }
        }
    }
}
