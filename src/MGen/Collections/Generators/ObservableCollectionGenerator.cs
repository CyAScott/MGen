using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections.Generators
{
    class ObservableCollectionDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ObservableCollectionGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.ObjectModel.Collection`1",
            "System.Collections.ObjectModel.ObservableCollection`1"
        };
    }

    class ObservableCollectionGenerator : ListGenerator
    {
        public ObservableCollectionGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasAdd => true;
        public override bool HasComparer => false;
        public override bool HasToArray => false;
    }
}
