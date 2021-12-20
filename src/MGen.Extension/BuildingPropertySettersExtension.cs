using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingPropertySettersExtension : IHandleBuildingPropertySetters
    {
        public void Handle(PropertySetterBuilderContext context, Action next)
        {
            next();
        }
    }
}
