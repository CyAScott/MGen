using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class OrderedDictionaryDetector : CollectionTypeDetector
    {
        public OrderedDictionaryDetector(GeneratorExecutionContext context) =>
            OrderDictionary = context.Compilation.GetTypeByMetadataName("System.Collections.Specialized.OrderedDictionary") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Specialized.OrderedDictionary");

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new OrderedDictionaryGenerator(context, type, OrderDictionary, variableName);

        public ITypeSymbol OrderDictionary { get; }

        public override string[] Types => new[]
        {
            "System.Collections.Specialized.IOrderedDictionary",
            "System.Collections.Specialized.OrderedDictionary"
        };
    }

    class OrderedDictionaryGenerator : HashtableGenerator
    {
        public OrderedDictionaryGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
        }

        public override bool HasComparer => false;
        public override bool HasToArray => false;
    }
}
