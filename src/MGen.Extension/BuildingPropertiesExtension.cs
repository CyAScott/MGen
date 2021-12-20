using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingPropertiesExtension : IHandleBuildingProperties
    {
        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();
        }
    }
}
