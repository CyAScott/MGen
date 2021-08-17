using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections.Generators
{
    class ConcurrentBagDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ConcurrentBagGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Concurrent.ConcurrentBag`1"
        };
    }

    class ConcurrentBagGenerator : ListGenerator
    {
        public ConcurrentBagGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasAdd => true;
        public override bool HasGet => false;
        public override bool HasSet => false;
    }
}
