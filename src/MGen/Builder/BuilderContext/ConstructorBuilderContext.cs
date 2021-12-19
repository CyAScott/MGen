using System;

namespace MGen.Builder.BuilderContext
{
    public class ConstructorBuilderContext : ClassBuilderContext
    {
        internal ConstructorBuilderContext(ClassBuilderContext context, ConstructorBuilder constructor)
            : base(context)
        {
            Constructor = constructor;
        }

        public ConstructorBuilder Constructor { get; }
    }

    public interface IHandleBuildingConstructors : IAmAnExtension
    {
        public void Handle(ConstructorBuilderContext context, Action next);
    }
}
