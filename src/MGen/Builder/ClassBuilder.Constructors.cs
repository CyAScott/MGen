using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using System.Collections.Generic;

namespace MGen.Builder
{
    public partial interface IClassBuilder
    {
        /// <summary>
        /// Appends a constructor to the class.
        /// </summary>
        IClassBuilder AppendConstructor(ClassBuilderContext context, ConstructorBuilder constructor);
    }

    partial class ClassBuilder
    {
        IHandleBuildingConstructors[] BuildConstructorBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingConstructors>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingConstructors handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WriteReadOnlyConstructor.Instance);
            list.Add(WriteDefaultConstructor.Instance);
            return list.ToArray();
        }

        protected IHandleBuildingConstructors[] ConstructorBuilders { get; }

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
