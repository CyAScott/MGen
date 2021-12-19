using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        IHandleBuildingPropertySetters[] BuildPropertySetterBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingPropertySetters>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingPropertySetters handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WritePropertyBinders.Instance);
            list.Add(WritePropertyDefaultGetterAndSetter.Instance);
            return list.ToArray();
        }

        protected IHandleBuildingPropertySetters[] PropertySetterBuilders { get; }

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
