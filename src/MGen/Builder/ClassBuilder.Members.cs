using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MGen.Builder
{
    partial class ClassBuilder
    {
        protected void AppendClassMembers(ClassBuilderContext context, InterfaceInfo @interface)
        {
            var membrNames = new HashSet<string>();

            foreach (var memberInfo in @interface.Values)
            {
                var @explicit = false;
                var symbols = memberInfo.Symbols;

                var secondaryPropertyIndex = memberInfo.IndexOfSecondaryProperty();
                var secondaryProperty = secondaryPropertyIndex == -1 ? null : (IPropertySymbol)symbols[secondaryPropertyIndex];

                for (var index = 0; index < symbols.Count; index++)
                {
                    if (index != secondaryPropertyIndex)
                    {
                        var symbol = symbols[index];

                        membrNames.Add(symbol.Name);

                        switch (symbol)
                        {
                            case IEventSymbol @event:
                                AppendEvent(new EventBuilderContext(context, @event, @explicit));
                                break;

                            case IMethodSymbol method:
                                AppendMethod(new MethodBuilderContext(context, method, @explicit));
                                break;

                            case IPropertySymbol primaryProperty:

                                if (index == 0)
                                {
                                    string? fieldName = null;
                                    if (!primaryProperty.IsIndexer)
                                    {
                                        fieldName = "_" + primaryProperty.Name;
                                        while (membrNames.Contains(fieldName))
                                        {
                                            fieldName = "_" + fieldName;
                                        }

                                        membrNames.Add(fieldName);
                                    }

                                    AppendProperty(new PropertyBuilderContext(context, primaryProperty, @explicit, secondaryProperty, fieldName));
                                }
                                else
                                {
                                    AppendProperty(new PropertyBuilderContext(context, primaryProperty, @explicit));
                                }

                                break;
                        }
                    }

                    @explicit = true;
                }
            }
        }
    }
}
