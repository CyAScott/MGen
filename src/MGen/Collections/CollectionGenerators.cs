using MGen.Builder.BuilderContext;
using MGen.Collections.Generators;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MGen.Collections
{
    public class CollectionGenerators
    {
        public CollectionGenerators(GeneratorExecutionContext context)
        {
            void Add(CollectionTypeDetector generator)
            {
                foreach (var fullName in generator.Types)
                {
                    var type = context.Compilation.GetTypeByMetadataName(fullName);
                    if (type != null)
                    {
                        Generators[type.ContainingAssembly + "." + type.ContainingNamespace + "." + type.MetadataName] = generator;
                    }
                }
            }

            Add(new ArrayClassDetector());
            Add(new ArrayListDetector(context));
            Add(new BitArrayDetector());
            Add(new BlockingCollectionDetector());
            Add(new ConcurrentBagDetector());
            Add(new ConcurrentDictionaryDetector());
            Add(new DictionaryDetector(context));
            Add(new HashtableDetector(context));
            Add(new LinkedListDetector());
            Add(new ListDetector(context));
            Add(new NameValueCollectionDetector(context));
            Add(new StringCollectionDetector());
            Add(new ObservableCollectionDetector());
            Add(new OrderedDictionaryDetector(context));
            Add(new ReadOnlyObservableCollectionDetector(context));
            Add(new QueueDetector());
            Add(new SetDetector(context));
            Add(new SortedDictionaryDetector());
            Add(new SortedListDetector());
            Add(new SortedSetDetector());
            Add(new StackDetector());
            Add(new StringCollectionDetector());
            Add(new StringDictionaryDetector());
        }

        public Dictionary<string, CollectionTypeDetector> Generators { get; } = new();

        public bool TryToGet(ClassBuilderContext context, ITypeSymbol type, string variableName, out CollectionGenerator generator)
        {
            if (type is IArrayTypeSymbol arrayType)
            {
                generator = new ArrayGenerator(context, arrayType, variableName);
                return true;
            }

            var key = type.ContainingAssembly + "." + type.ContainingNamespace + "." + type.MetadataName;

            if (!Generators.TryGetValue(key, out var detector))
            {
                generator = default!;
                return false;
            }

            generator = detector.Create(context, type, variableName);

            return true;
        }
    }
}
