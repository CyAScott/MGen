using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class BitArrayDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new BitArrayGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.BitArray"
        };
    }

    class BitArrayGenerator : ArrayListGenerator
    {
        public BitArrayGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
            ValueType = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.Boolean") ?? throw new InvalidOperationException("Unable to resolve System.Boolean");
        }

        public override ITypeSymbol ValueType { get; }
        public override bool HasAdd => false;
        public override bool HasComparer => false;
        public override bool HasToArray => false;
    }
}
