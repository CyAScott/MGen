using System;

namespace MGen.Builder.BuilderContext
{
    public class PropertySetterBuilderContext : PropertyBuilderContext
    {
        internal PropertySetterBuilderContext(PropertyBuilderContext context)
            : base(context, context.Primary, context.Explicit, context.Secondary, context.FieldName)
        {
        }
    }

    public interface IHandleBuildingPropertySetters
    {
        public void Handle(PropertySetterBuilderContext context, Action next);
    }
}
