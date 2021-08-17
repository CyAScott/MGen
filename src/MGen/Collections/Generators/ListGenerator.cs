using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class ListDetector : CollectionTypeDetector
    {
        public ListDetector(GeneratorExecutionContext context) =>
            List = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.List`1") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Generic.List`1");

        public ITypeSymbol List { get; }

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ListGenerator(context, type, List, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.ICollection`1",
            "System.Collections.Generic.IEnumerable`1",
            "System.Collections.Generic.IList`1",
            "System.Collections.Generic.IReadOnlyCollection`1",
            "System.Collections.Generic.IReadOnlyList`1",
            "System.Collections.Generic.List`1"
        };
    }

    class ListGenerator : ArrayListGenerator
    {
        public ListGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
            HasToArray = type.Name == "List";
        }

        public override bool HasAdd => true;
        public override bool HasToArray { get; }
    }
}
