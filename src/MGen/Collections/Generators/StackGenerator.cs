using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class StackDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new StackGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Concurrent.ConcurrentStack`1",
            "System.Collections.Generic.Stack`1",
            "System.Collections.Stack"
        };
    }

    partial class StackGenerator : ListGenerator
    {
        public StackGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)

        {
        }

        public override bool HasGet => false;
        public override bool HasSet => false;
        public override bool HasToArray => true;
    }

    partial class StackGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var elementValue = "_0_" + variablePostFix;

            if (TypeArguments.Length == 0)
            {
                Builder.AppendLine(builder => builder.Append("foreach (var ").Append(elementValue).Append(" in ")
                    .Append("System.Linq.Enumerable.Reverse(System.Linq.Enumerable.Cast<object>(").Append(InternalName).Append(")))"));
            }
            else
            {
                Builder.AppendLine(builder => builder.Append("foreach (var ").Append(elementValue).Append(" in ")
                    .Append("System.Linq.Enumerable.Reverse(").Append(InternalName).Append("))"));
            }

            Builder.OpenBrace();

            body(this, elementValue);

            Builder.CloseBrace();

            return this;
        }
    }

    partial class StackGenerator
    {
        public override bool HasAdd => true;

        public override CollectionGenerator Add(Action<CollectionGenerator> value)
        {
            Builder.AppendIndent().String.Append(InternalName).Append(".Push(");
            value(this);
            Builder.AppendLine(");");

            return this;
        }
    }
}
