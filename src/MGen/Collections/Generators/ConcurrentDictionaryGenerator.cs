using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class ConcurrentDictionaryDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ConcurrentDictionaryGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Concurrent.ConcurrentDictionary`2"
        };
    }

    partial class ConcurrentDictionaryGenerator : DictionaryGenerator
    {
        public ConcurrentDictionaryGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasComparer => false;
    }

    partial class ConcurrentDictionaryGenerator
    {
        public override CollectionGenerator Add(Action<CollectionGenerator> key, Action<CollectionGenerator> value)
        {
            var builder = Builder.AppendIndent().String.Append(InternalName).Append(".TryAdd(");
            key(this);
            builder.Append(", ");
            value(this);
            Builder.AppendLine(");");

            return this;
        }
    }
}
