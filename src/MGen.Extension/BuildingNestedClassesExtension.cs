using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingNestedClassesExtension : IHandleBuildingNestedClasses
    {
        public void Handle(NestedClassBuilderContext context, Action next)
        {
            next();
        }
    }
}
