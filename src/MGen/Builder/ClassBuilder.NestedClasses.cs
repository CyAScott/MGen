using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected Dictionary<string, ITypeSymbol> NestedClasses { get; } = new();

        protected HashSet<string> WrittenClasses { get; } = new HashSet<string>();

        protected IHandleBuildingNestedClasses[] NestedClassBuilders { get; } = new IHandleBuildingNestedClasses[]
        {
            WriteDefaultClass.Instance,
            WriteCloneSupport.Instance,
            WriteNetSerialization.Instance,
            WritePropertyBinders.Instance,
            WriteReadOnlyConstructor.Instance,
            WriteDefaultConstructor.Instance
        };

        protected void AppendNestedClasses(ClassBuilderContext context)
        {
            if (NestedClassBuilders.Length == 0 || NestedClasses.Count == 0)
            {
                WrittenClasses.UnionWith(NestedClasses.Keys);
                return;
            }

            foreach (var pair in NestedClasses)
            {
                if (WrittenClasses.Add(pair.Key))
                {
                    AppendLine();
                    AppendNestedClass(
                        new NestedClassBuilderContext(context, pair.Value, pair.Key),
                        new InterfaceInfo(
                            $"{context.Namespace}.{pair.Key}",
                            pair.Value,
                            context.Modifiers,
                            pair.Value.GetMGenAttributes()));
                }
            }
        }

        public string Append(ClassBuilderContext context, ITypeSymbol @interface)
        {
            var className = context.GenerateAttribute.DestinationNamePattern.GetDestinationName(context.GenerateAttribute.SourceNamePattern ?? "", @interface) ?? "";
            NestedClasses[className] = @interface;
            return className;
        }

        protected void AppendNestedClass(NestedClassBuilderContext context, InterfaceInfo @interface, int index = 0) =>
            NestedClassBuilders[index].Handle(context,
                index == NestedClassBuilders.Length - 1 ?
                () => AppendClassMembers(context, @interface) :
                () => AppendNestedClass(context, @interface, index + 1));
    }
}
