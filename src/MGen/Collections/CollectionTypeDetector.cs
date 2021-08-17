using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections
{
    public abstract class CollectionTypeDetector
    {
        /// <summary>
        /// Creates an instance of the <see cref="CollectionGenerator"/>.
        /// </summary>
        public abstract CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName);

        /// <summary>
        /// The full named path for the type.
        /// </summary>
        public abstract string[] Types { get; }
    }
}
