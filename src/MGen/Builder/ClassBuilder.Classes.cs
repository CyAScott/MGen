using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected IHandleBuildingClasses[] ClassBuilders { get; } = new IHandleBuildingClasses[]
        {
            WriteCloneSupport.Instance,
            WriteConversionSupport.Instance,
            WriteNetSerialization.Instance,
            WritePropertyBinders.Instance,
            WriteDefaultClass.Instance,
            WriteReadOnlyConstructor.Instance,
            WriteDefaultConstructor.Instance
        };

        public ClassBuilderContext AppendClass(InterfaceInfo @interface, GenerateAttributeRuntime generateAttribute,
            GeneratorExecutionContext generatorExecutionContext, Collections.CollectionGenerators collectionGenerators)
        {
            var context = new ClassBuilderContext(
                generateAttribute,
                @interface.Modifiers,
                @interface.Type,
                string.Join(".", @interface.Path.Take(@interface.Path.Length - 1)),
                generateAttribute.DestinationNamePattern.GetDestinationName(generateAttribute.SourceNamePattern, @interface.Type),
                this,
                generatorExecutionContext,
                collectionGenerators);

            String.Append("namespace ").AppendLine(context.Namespace);
            OpenBrace();

            WrittenClasses.Add(context.ClassName);

            if (ClassBuilders.Length > 0)
            {
                var originalIndentLevel = IndentLevel;

                AppendClass(context, @interface);

                CloseBrace(IndentLevel - originalIndentLevel);
            }

            AppendNestedClasses(context);

            CloseBrace();

            return context;
        }

        protected void AppendClass(ClassBuilderContext context, InterfaceInfo @interface, int index = 0) =>
            ClassBuilders[index].Handle(context,
                index == ClassBuilders.Length - 1 ?
                () => AppendClassMembers(context, @interface) :
                () => AppendClass(context, @interface, index + 1));
    }
}
