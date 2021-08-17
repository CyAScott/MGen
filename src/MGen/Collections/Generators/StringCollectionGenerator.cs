using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class StringCollectionDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new StringCollectionGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Specialized.StringCollection"
        };
    }

    class StringCollectionGenerator : ArrayListGenerator
    {
        public StringCollectionGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
            ValueType = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Unable to resolve System.String");
        }

        public override ITypeSymbol ValueType { get; }
        public override bool HasAdd => true;
        public override bool HasComparer => false;
        public override bool HasToArray => false;
    }
}
