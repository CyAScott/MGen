using Microsoft.CodeAnalysis;
using System;

namespace MGen.Builder.BuilderContext
{
    public class NestedClassBuilderContext : ClassBuilderContext
    {
        internal NestedClassBuilderContext(ClassBuilderContext context, ITypeSymbol @interface, string className)
            : base(context.GenerateAttribute, context.Modifiers, @interface, context.Namespace, className, context.Builder, context.GeneratorExecutionContext, context.CollectionGenerators)
        {
        }
    }

    public interface IHandleBuildingNestedClasses : IAmAnExtension
    {
        public void Handle(NestedClassBuilderContext context, Action next);
    }
}
