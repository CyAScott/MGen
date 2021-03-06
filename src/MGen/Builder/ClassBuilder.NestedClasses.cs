using MGen.Builder.BuilderContext;
using MGen.Builder.Writers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MGen.Builder
{
    public partial interface IClassBuilder
    {
        /// <summary>
        /// Adds a nested class to implement.
        /// </summary>
        string Append(ClassBuilderContext context, ITypeSymbol @interface);

        /// <summary>
        /// Gets the class name for a nested class.
        /// </summary>
        bool TryGetNameForImplementation(ITypeSymbol @interface, out string? name);
    }

    partial class ClassBuilder
    {
        protected Dictionary<string, ITypeSymbol> NestedClasses { get; } = new();

        protected HashSet<string> WrittenClasses { get; } = new();

        protected IHandleBuildingNestedClasses[] NestedClassBuilders { get; } = new IHandleBuildingNestedClasses[]
        {
            WriteCloneSupport.Instance,
            WriteConversionSupport.Instance,
            WriteNetSerialization.Instance,
            WritePropertyBinders.Instance,
            WriteDefaultClass.Instance,
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

                    var originalIndentLevel = IndentLevel;

                    AppendNestedClass(
                        new NestedClassBuilderContext(context, pair.Value, pair.Key),
                        new InterfaceInfo(
                            $"{context.Namespace}.{pair.Key}",
                            pair.Value,
                            context.Modifiers,
                            pair.Value.GetMGenAttributes()));

                    CloseBrace(IndentLevel - originalIndentLevel);
                }
            }
        }

        public string Append(ClassBuilderContext context, ITypeSymbol @interface)
        {
            var className = context.GenerateAttribute.DestinationNamePattern.GetDestinationName(context.GenerateAttribute.SourceNamePattern, @interface);
            NestedClasses[className] = @interface;
            return className;
        }

        public bool TryGetNameForImplementation(ITypeSymbol @interface, out string? name)
        {
            foreach (var pair in NestedClasses)
            {
                if (SymbolEqualityComparer.Default.Equals(@interface, pair.Value))
                {
                    name = pair.Key;
                    return true;
                }
            }

            name = null;
            return false;
        }

        protected void AppendNestedClass(NestedClassBuilderContext context, InterfaceInfo @interface, int index = 0) =>
            NestedClassBuilders[index].Handle(context,
                index == NestedClassBuilders.Length - 1 ?
                () => AppendClassMembers(context, @interface) :
                () => AppendNestedClass(context, @interface, index + 1));
    }
}
