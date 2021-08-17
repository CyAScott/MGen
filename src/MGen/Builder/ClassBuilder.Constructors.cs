using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingConstructors[] ConstructorBuilders { get; } = new IHandleBuildingConstructors[]
        {
            WriteReadOnlyConstructor.Instance,
            WriteDefaultConstructor.Instance
        };

        public IClassBuilder AppendConstructor(ClassBuilderContext context, ConstructorBuilder constructor) =>
            AppendConstructor(new ConstructorBuilderContext(context, constructor));

        protected IClassBuilder AppendConstructor(ConstructorBuilderContext context, int index = 0)
        {
            if (index < ConstructorBuilders.Length)
            {
                ConstructorBuilders[index].Handle(context, () => AppendConstructor(context, index + 1));
            }
            return this;
        }
    }
}
