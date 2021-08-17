using System;

namespace MGen.Builder.BuilderContext
{
    public class PropertyGetterBuilderContext : PropertyBuilderContext
    {
        internal PropertyGetterBuilderContext(PropertyBuilderContext context)
            : base(context, context.Primary, context.Explicit, context.Secondary, context.FieldName)
        {
        }
    }

    public interface IHandleBuildingPropertyGetters
    {
        public void Handle(PropertyGetterBuilderContext context, Action next);
    }
}
