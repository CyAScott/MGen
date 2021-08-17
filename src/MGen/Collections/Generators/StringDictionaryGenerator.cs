using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class StringDictionaryDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new StringDictionaryGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Specialized.StringDictionary"
        };
    }

    partial class StringDictionaryGenerator : HashtableGenerator
    {
        public StringDictionaryGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
            KeyType = ValueType = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Unable to resolve System.String");
        }

        public override ITypeSymbol KeyType { get; }
        public override ITypeSymbol ValueType { get; }
        public override bool HasComparer => false;
        public override bool HasToArray => false;
    }

    partial class StringDictionaryGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var key = "_0_" + variablePostFix;

            Builder.AppendLine(builder => builder.Append("foreach (string ").Append(key).Append(" in ").Append(InternalName).Append(".Keys)"));

            Builder.OpenBrace();

            body(this, InternalName + "[" + key + "]", key);

            Builder.CloseBrace();

            return this;
        }
    }
}
