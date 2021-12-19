using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        IHandleBuildingClasses[] BuildClassBuilders(params IAmAnExtension[] extensions)
        {
            var list = new List<IHandleBuildingClasses>();
            foreach (var extension in extensions)
            {
                if (extension is IHandleBuildingClasses handler)
                {
                    list.Add(handler);
                }
            }
            list.Add(WriteCloneSupport.Instance);
            list.Add(WriteConversionSupport.Instance);
            list.Add(WriteNetSerialization.Instance);
            list.Add(WritePropertyBinders.Instance);
            list.Add(WriteDefaultClass.Instance);
            list.Add(WriteReadOnlyConstructor.Instance);
            list.Add(WriteDefaultConstructor.Instance);
            return list.ToArray();
        }

        protected IHandleBuildingClasses[] ClassBuilders { get; }

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
