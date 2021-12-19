using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingProperties[] PropertyBuilders { get; }
        IHandleBuildingProperties[] BuildPropertyBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingProperties>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingProperties handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WriteCloneSupport.Instance);
            list.Add(WriteConversionSupport.Instance);
            list.Add(WriteNetSerialization.Instance);
            list.Add(WriteReadOnlyConstructor.Instance);
            list.Add(WriteNestedClass.Instance);
            list.Add(WriteDefaultProperty.Instance);
            return list.ToArray();
        }

        protected void AppendProperty(PropertyBuilderContext context, int index = 0)
        {
            if (index < PropertyBuilders.Length)
            {
                PropertyBuilders[index].Handle(context,
                    index == PropertyBuilders.Length - 1 ?
                    () => AppendPropertyGetter(new PropertyGetterBuilderContext(context)) :
                    () => AppendProperty(context, index + 1));
            }
        }
    }
}
