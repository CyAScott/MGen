using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingProperties[] PropertyBuilders { get; } = new IHandleBuildingProperties[]
        {
            WriteCloneSupport.Instance,
            WriteConversionSupport.Instance,
            WriteNetSerialization.Instance,
            WriteReadOnlyConstructor.Instance,
            WriteNestedClass.Instance,
            WriteDefaultProperty.Instance
        };

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
