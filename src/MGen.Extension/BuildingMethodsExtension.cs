using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingMethodsExtension : IHandleBuildingMethods
    {
        public void Handle(MethodBuilderContext context, Action next)
        {
            next();
        }
    }
}
