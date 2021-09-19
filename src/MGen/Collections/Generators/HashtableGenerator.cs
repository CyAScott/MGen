using MGen.Builder;
using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class HashtableDetector : CollectionTypeDetector
    {
        public HashtableDetector(GeneratorExecutionContext context) =>
            Hashtable = context.Compilation.GetTypeByMetadataName("System.Collections.Hashtable") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Hashtable");

        public ITypeSymbol Hashtable { get; }

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new HashtableGenerator(context, type, Hashtable, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Hashtable",
            "System.Collections.IDictionary"
        };
    }

    partial class HashtableGenerator : CollectionGenerator
    {
        public HashtableGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
            KeyType = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.Object") ?? throw new InvalidOperationException("Unable to resolve System.Object");
        }

        public override ITypeSymbol KeyType { get; }
        public override bool HasComparer => false;
    }

    partial class HashtableGenerator
    {
        public override CollectionGenerator Create(CollectionGenerator? source = null)
        {
            var builder = Builder.AppendIndent().String
                .Append("var ").Append(InternalName).Append(" = new ").Append(Implementation.ContainingNamespace).Append('.').Append(Implementation.Name);

            if (TypeArguments.Length == 2)
            {
                builder.Append('<').AppendType(TypeArguments[0]).Append(", ").AppendType(TypeArguments[1]).Append(">(");
            }
            else
            {
                builder.Append("(");
            }

            if (source?.HasComparer == true)
            {
                builder.Append(source.GetComparer());
            }

            Builder.AppendLine(");");

            return this;
        }
    }

    partial class HashtableGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var keyValuePair = "_0_" + variablePostFix;

            Builder
                .AppendLine(builder => builder.Append("var ").Append(keyValuePair).Append(" = ").Append(InternalName).Append(".GetEnumerator();"))
                .AppendLine(builder => builder.Append("while (").Append(keyValuePair).Append(".MoveNext())"))
                .OpenBrace();

            body(this, keyValuePair + ".Value", keyValuePair + ".Key");

            Builder.CloseBrace();

            return this;
        }
    }

    partial class HashtableGenerator
    {
        public override bool HasAdd => true;
        public override bool HasKeys => true;

        public override CollectionGenerator Add(Action<CollectionGenerator> key, Action<CollectionGenerator> value)
        {
            var builder = Builder.Append(InternalName).String.Append(".Add(");
            key(this);
            builder.Append(", ");
            value(this);
            Builder.AppendLine(");");
            return this;
        }
    }

    partial class HashtableGenerator
    {
        public override bool HasGet => true;

        public override CollectionGenerator Get(string[] indices, string? preFix = null, string? postFix = null)
        {
            Builder.Append(preFix).String.Append(InternalName).Append('[').Append(indices[0]).Append(']').Append(postFix);

            return this;
        }
    }

    partial class HashtableGenerator
    {
        public override bool HasLength => true;

        public override string Length(int dimension = 0) => InternalName + ".Count";
    }

    partial class HashtableGenerator
    {
        public override bool HasSet => true;

        public override CollectionGenerator Set(string[] indices, Action<CollectionGenerator> value)
        {
            Builder.Append(InternalName).String.Append('[').Append(indices[0]).Append("]  = ");
            value(this);
            Builder.AppendLine(";");

            return this;
        }
    }

    partial class HashtableGenerator
    {
        public override bool HasToArray => false;
    }
}
