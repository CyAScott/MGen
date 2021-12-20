using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingConstructorsExtension : IHandleBuildingConstructors
    {
        public void Handle(ConstructorBuilderContext context, Action next)
        {
            next();
        }
    }
}
