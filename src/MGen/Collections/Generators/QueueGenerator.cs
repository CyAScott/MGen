using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class QueueDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new QueueGenerator(context, type, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.Concurrent.ConcurrentQueue`1",
            "System.Collections.Generic.Queue`1",
            "System.Collections.Queue"
        };
    }

    partial class QueueGenerator : ListGenerator
    {
        public QueueGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }

        public override bool HasGet => false;
        public override bool HasSet => false;
        public override bool HasToArray => true;
    }

    partial class QueueGenerator
    {
        public override bool HasAdd => true;

        public override CollectionGenerator Add(Action<CollectionGenerator> value)
        {
            Builder.Append(InternalName).String.Append(".Enqueue(");
            value(this);
            Builder.AppendLine(");");

            return this;
        }
    }
}
