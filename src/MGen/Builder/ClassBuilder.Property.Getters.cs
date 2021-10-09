using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingPropertyGetters[] PropertyGetterBuilders { get; } =
        {
            WritePropertyDefaultGetterAndSetter.Instance
        };

        protected void AppendPropertyGetter(PropertyGetterBuilderContext context)
        {
            if (context.HasGet)
            {
                AppendLine("get");
                OpenBrace();
                AppendPropertyGetter(context, 0);
                CloseBrace();
            }
            AppendPropertySetter(new PropertySetterBuilderContext(context));
        }

        protected void AppendPropertyGetter(PropertyGetterBuilderContext context, int index)
        {
            if (index < PropertyGetterBuilders.Length)
            {
                PropertyGetterBuilders[index].Handle(context, () => AppendPropertyGetter(context, index + 1));
            }
        }
    }
}
