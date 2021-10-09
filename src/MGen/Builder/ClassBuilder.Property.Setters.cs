using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingPropertySetters[] PropertySetterBuilders { get; } =
        {
            WritePropertyBinders.Instance,
            WritePropertyDefaultGetterAndSetter.Instance
        };

        protected void AppendPropertySetter(PropertySetterBuilderContext context)
        {
            if (context.HasSet)
            {
                AppendLine("set");
                OpenBrace();
                AppendPropertySetter(context, 0);
                CloseBrace();
            }
        }

        protected void AppendPropertySetter(PropertySetterBuilderContext context, int index)
        {
            if (index < PropertySetterBuilders.Length)
            {
                PropertySetterBuilders[index].Handle(context, () => AppendPropertySetter(context, index + 1));
            }
        }
    }
}
