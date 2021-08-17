using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class ArrayClassDetector : CollectionTypeDetector
    {
        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ArrayClassGenerator(context, type, variableName);

        public override string[] Types => new[] { "System.Array" };
    }

    partial class ArrayClassGenerator : CollectionGenerator
    {
        public ArrayClassGenerator(ClassBuilderContext context, ITypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }
    }

    partial class ArrayClassGenerator
    {
        public override CollectionGenerator Create(CollectionGenerator? source = null)
        {
            var builder = Builder.AppendIndent().String
                .Append("var ").Append(InternalName).Append(" = System.Array.CreateInstance(typeof(object), ");

            if (source == null)
            {
                builder.Append("0");
            }
            else
            {
                builder
                    .Append("MGen.ArrayHelper.GetLengths(").Append(source.InternalName).Append("), ")
                    .Append("MGen.ArrayHelper.GetLowerBounds(").Append(source.InternalName).Append(')');
            }

            Builder.AppendLine(");");

            return this;
        }
    }

    partial class ArrayClassGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var indices = "_0_" +  variablePostFix;
            var value = "_1_" + variablePostFix;

            Builder.AppendLine(builder => builder.Append("foreach (int[] ").Append(indices).Append(" in MGen.ArrayHelper.GetIndices(").Append(InternalName).Append("))"));
            Builder.OpenBrace();

            Builder.AppendLine(builder => builder.Append("var ").Append(value).Append(" = ").Append(InternalName).Append(".GetValue(").Append(indices).Append(");"));

            body(this, value, indices);

            Builder.CloseBrace();

            return this;
        }
    }

    partial class ArrayClassGenerator
    {
        public override bool HasGet => true;

        public override CollectionGenerator Get(string[] indices, string? preFix = null, string? postFix = null)
        {
            var builder = Builder.Append(preFix).String.Append(InternalName).Append(".GetValue(");

            for (var index = 0; index < indices.Length; index++)
            {
                builder.Append(", ");
                builder.Append(indices[index]);
            }

            builder.Append(')').Append(postFix);

            return this;
        }
    }

    partial class ArrayClassGenerator
    {
        public override bool HasLength => true;

        public override int Rank => 1;//NOTE: Array rank can only be discovered at runtime

        public override string Length(int dimension = 0) => InternalName + ".GetLength(" + dimension + ")";
    }

    partial class ArrayClassGenerator
    {
        public override bool HasSet => true;

        public override CollectionGenerator Set(string[] indices, Action<CollectionGenerator> value)
        {
            var builder = Builder.AppendIndent().String
                .Append(InternalName).Append(".SetValue(");

            value(this);

            for (var index = 0; index < indices.Length; index++)
            {
                builder.Append(", ").Append(indices[index]);
            }

            Builder.AppendLine(");");

            return this;
        }
    }

    partial class ArrayClassGenerator
    {
        public override CollectionGenerator ToArray(string? preFix = null, string? postFix = null)
        {
            Builder.Append(preFix).String.Append("System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Cast<object>(").Append(InternalName).Append("))").Append(postFix);

            return this;
        }
    }
}
