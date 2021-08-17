using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class LinkedListDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new LinkedListGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Generic.LinkedList`1"
        };
    }

    partial class LinkedListGenerator : ListGenerator
    {
        public LinkedListGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasAdd => true;
        public override bool HasGet => false;
        public override bool HasSet => false;
        public override bool HasToArray => false;
    }

    partial class LinkedListGenerator
    {
        public override CollectionGenerator Add(Action<CollectionGenerator> value)
        {
            Builder.Append(InternalName).String.Append(".AddLast(");
            value(this);
            Builder.AppendLine(");");

            return this;
        }
    }
}
