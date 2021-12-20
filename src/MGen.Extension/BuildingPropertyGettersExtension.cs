using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingPropertyGettersExtension : IHandleBuildingPropertyGetters
    {
        public void Handle(PropertyGetterBuilderContext context, Action next)
        {
            next();
        }
    }
}
