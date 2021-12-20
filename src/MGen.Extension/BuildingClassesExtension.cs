using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingClassesExtension : IHandleBuildingClasses
    {
        public void Handle(ClassBuilderContext context, Action next)
        {
            next();
        }
    }
}
