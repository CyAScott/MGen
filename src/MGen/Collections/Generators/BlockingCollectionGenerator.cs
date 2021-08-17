using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections.Generators
{
    class BlockingCollectionDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new BlockingCollectionGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Concurrent.BlockingCollection`1"
        };
    }

    class BlockingCollectionGenerator : ListGenerator
    {
        public BlockingCollectionGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasAdd => true;
        public override bool HasGet => false;
        public override bool HasSet => false;
    }
}
