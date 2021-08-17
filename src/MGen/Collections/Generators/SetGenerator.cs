using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class SetDetector : CollectionTypeDetector
    {
        public SetDetector(GeneratorExecutionContext context) =>
            Set = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.HashSet`1") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Generic.HashSet`1");

        public ITypeSymbol Set { get; }

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new SetGenerator(context, type, Set, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.HashSet`1",
            "System.Collections.Generic.IReadOnlySet`1",
            "System.Collections.Generic.ISet`1"
        };
    }

    class SetGenerator : ListGenerator
    {
        public SetGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
            HasToArray = type.Name == "HashSet";
        }

        public override bool HasAdd => true;
        public override bool HasToArray { get; }
    }
}
