using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class DictionaryDetector : CollectionTypeDetector
    {
        public DictionaryDetector(GeneratorExecutionContext context) =>
            Dictionary = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.Dictionary`2") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Generic.Dictionary`2");

        public ITypeSymbol Dictionary { get; }

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new DictionaryGenerator(context, type, Dictionary, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.Dictionary`2",
            "System.Collections.Generic.IDictionary`2",
            "System.Collections.Generic.IReadOnlyDictionary`2"
        };
    }

    partial class DictionaryGenerator : HashtableGenerator
    {
        public DictionaryGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
            KeyType = TypeArguments.Length > 0 ? TypeArguments[0] : GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.Object") ?? throw new InvalidOperationException("Unable to resolve System.Object");
            HasComparer = type.Name == "Dictionary";
        }

        public override ITypeSymbol KeyType { get; }
        public override bool HasComparer { get; }
    }

    partial class DictionaryGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var keyValuePair = "_0_" + variablePostFix;

            Builder
                .AppendLine(builder => builder.Append("foreach (var ").Append(keyValuePair).Append(" in ").Append(InternalName).Append(')'))
                .OpenBrace();

            body(this, keyValuePair + ".Value", keyValuePair + ".Key");

            Builder.CloseBrace();

            return this;
        }
    }
}
