using MGen.Builder.BuilderContext;
using System;

namespace MGen
{
    public class BuildingEventsExtension : IHandleBuildingEvents
    {
        public void Handle(EventBuilderContext context, Action next)
        {
            next();
        }
    }
}
