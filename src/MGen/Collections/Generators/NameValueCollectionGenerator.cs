using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class NameValueCollectionDetector : CollectionTypeDetector
    {
        public NameValueCollectionDetector(GeneratorExecutionContext context) =>
            NameValueCollection = context.Compilation.GetTypeByMetadataName("System.Collections.Specialized.NameValueCollection") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Specialized.NameValueCollection");

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new NameValueCollectionGenerator(context, type, NameValueCollection, variableName);

        public ITypeSymbol NameValueCollection { get; }

        public override string[] Types => new[]
        {
            "System.Collections.Specialized.NameValueCollection"
        };
    }

    partial class NameValueCollectionGenerator : HashtableGenerator
    {
        public NameValueCollectionGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
            HasAdd = type.Name == "NameValueCollection";
            KeyType = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Unable to resolve System.String");
            ValueType = HasAdd ? KeyType : base.ValueType!;
        }

        public override ITypeSymbol KeyType { get; }
        public override ITypeSymbol ValueType { get; }
        public override bool HasAdd { get; }
        public override bool HasComparer => false;
        public override bool HasToArray => false;
    }

    partial class NameValueCollectionGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var key = "_0_" + variablePostFix;

            Builder.AppendLine(builder => builder.Append("foreach (var ").Append(key).Append(" in ").Append(InternalName).Append(".AllKeys)"));

            Builder.OpenBrace();

            body(this, InternalName + "[" + key + "]", key);

            Builder.CloseBrace();

            return this;
        }
    }
}
