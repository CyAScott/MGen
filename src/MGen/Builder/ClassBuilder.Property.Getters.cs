using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        IHandleBuildingPropertyGetters[] BuildPropertyGetterBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingPropertyGetters>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingPropertyGetters handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WritePropertyDefaultGetterAndSetter.Instance);
            return list.ToArray();
        }

        protected IHandleBuildingPropertyGetters[] PropertyGetterBuilders { get; }

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
