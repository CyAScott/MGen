using Microsoft.CodeAnalysis;
using System;

namespace MGen.Builder.BuilderContext
{
    public class EventBuilderContext : MemberBuilderContext
    {
        internal EventBuilderContext(ClassBuilderContext context, IEventSymbol @event, bool @explicit)
            : base(context, @event, @explicit)
        {
        }

        /// <summary>
        /// The event that is being written.
        /// </summary>
        public IEventSymbol Event => (IEventSymbol)Member;
    }

    public interface IHandleBuildingEvents
    {
        public void Handle(EventBuilderContext context, Action next);
    }
}
