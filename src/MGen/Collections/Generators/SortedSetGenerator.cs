using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections.Generators
{
    class SortedSetDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new SortedSetGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.SortedSet`1"
        };
    }

    class SortedSetGenerator : ListGenerator
    {
        public SortedSetGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasAdd => true;
        public override bool HasToArray => true;
    }
}
