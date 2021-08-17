using MGen.Builder.BuilderContext;
using MGen.Collections.Generators;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace MGen.Collections
{
    /*
     * 
System.Collections.ObjectModel.KeyedCollection<TKey,TItem>
System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>
System.Collections.Specialized.ListDictionary
System.Collections.Specialized.StringDictionaryWithComparer
**System.Collections.CollectionBase
**System.Collections.DictionaryBase
**System.Collections.ReadOnlyCollectionBase
System.Collections.Immutable.*
System.Span<T>
System.ReadOnlySpan<T>
    Memory<T>, ReadOnlyMemory<T>, IMemoryOwner<T>
     */
    public class CollectionGenerators
    {
        public CollectionGenerators(GeneratorExecutionContext context)
        {
            void add(CollectionTypeDetector generator)
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

            add(new ArrayClassDetector());
            add(new ArrrayListDetector(context));
            add(new BitArrayDetector());
            add(new BlockingCollectionDetector());
            add(new ConcurrentBagDetector());
            add(new ConcurrentDictionaryDetector());
            add(new DictionaryDetector(context));
            add(new HashtableDetector(context));
            add(new LinkedListDetector());
            add(new ListDetector(context));
            add(new NameValueCollectionDetector(context));
            add(new StringCollectionDetector());
            add(new ObservableCollectionDetector());
            add(new OrderedDictionaryDetector(context));
            add(new ReadOnlyObservableCollectionDetector(context));
            add(new QueueDetector());
            add(new SetDetector(context));
            add(new SortedDictionaryDetector());
            add(new SortedListDetector());
            add(new SortedSetDetector());
            add(new StackDetector());
            add(new StringCollectionDetector());
            add(new StringDictionaryDetector());
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
                generator = null;
                return false;
            }

            generator = detector.Create(context, type, variableName);

            return true;
        }
    }
}
