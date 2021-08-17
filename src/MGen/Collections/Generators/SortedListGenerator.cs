using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;

namespace MGen.Collections.Generators
{
    class SortedListDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new SortedListGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.SortedList`2",
            "System.Collections.SortedList"
        };
    }

    partial class SortedListGenerator : DictionaryGenerator
    {
        public SortedListGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasComparer => false;
    }

    partial class SortedListGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            if (TypeArguments.Length > 0)
            {
                return base.Enumerate(variablePostFix, body);
            }

            var key = "_0_" + variablePostFix;
            var value = "_1_" + variablePostFix;

            Builder.AppendLine(builder => builder.Append("foreach (var ").Append(key).Append(" in ").Append(InternalName).Append(".Keys)"));

            Builder.OpenBrace();

            Builder.AppendLine(builder => builder.Append("var ").Append(value).Append(" = ").Append(InternalName).Append('[').Append(key).Append("];"));

            body(this, value, key);

            Builder.CloseBrace();

            return this;
        }
    }
}
