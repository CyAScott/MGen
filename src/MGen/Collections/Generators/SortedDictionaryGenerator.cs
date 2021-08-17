using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections.Generators
{
    class SortedDictionaryDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new SortedDictionaryGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.SortedDictionary`2"
        };
    }

    class SortedDictionaryGenerator : DictionaryGenerator
    {
        public SortedDictionaryGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasComparer => true;
    }
}
