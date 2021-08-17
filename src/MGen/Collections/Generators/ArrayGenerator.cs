using MGen.Builder;
using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    partial class ArrayGenerator : CollectionGenerator
    {
        public ArrayGenerator(ClassBuilderContext context, IArrayTypeSymbol type, string variableName)
            : base(context, type, type, variableName)
        {
        }
    }

    partial class ArrayGenerator
    {
        public override CollectionGenerator Create(CollectionGenerator? source = null)
        {
            var builder = Builder.AppendIndent().String
                .Append("var ").Append(InternalName).Append(" = new ");

            var valueType = ValueType.ToCsString();
            var indexOrBrace = valueType.IndexOf('[');

            builder.Append(indexOrBrace == -1 ? valueType : valueType.Substring(0, indexOrBrace)).Append('[');

            for (var dimension = 0; dimension < Rank; dimension++)
            {
                if (dimension > 0)
                {
                    builder.Append(", ");
                }

                if (source == null)
                {
                    builder.Append(0);
                }
                else
                {
                    builder.Append(source.Length(dimension));
                }
            }

            builder.Append(']');

            if (indexOrBrace != -1)
            {
                builder.Append(valueType.Substring(indexOrBrace));
            }

            Builder.AppendLine(";");

            return this;
        }
    }

    partial class ArrayGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            var elementValue = InternalName + "[";
            var indices = new string[Rank];

            for (var dimension = 0; dimension < Rank; dimension++)
            {
                var index = indices[dimension] = "_" + dimension + "_" + variablePostFix;

                if (dimension > 0)
                {
                    elementValue += ", " + index;
                }
                else
                {
                    elementValue += index;
                }

                Builder.AppendLine(builder => builder
                    .Append("for (int ").Append(index).Append(" = 0; ")
                    .Append(index).Append(" < ").Append(Length(dimension)).Append("; ")
                    .Append(index).Append("++)"))
                    .OpenBrace();
            }

            body(this, elementValue + "]", indices);

            Builder.CloseBrace(Rank);

            return this;
        }
    }

    partial class ArrayGenerator
    {
        public override bool HasGet => true;

        public override CollectionGenerator Get(string[] indices, string? preFix = null, string? postFix = null)
        {
            var builder = Builder.Append(preFix).String.Append(InternalName).Append('[');

            for (var index = 0; index < indices.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(indices[index]);
            }

            builder.Append(']').Append(postFix);

            return this;
        }
    }

    partial class ArrayGenerator
    {
        public override bool HasLength => true;

        public override string Length(int dimension = 0) => InternalName + (Rank == 1 ? ".Length" : ".GetLength(" + dimension + ")");
    }

    partial class ArrayGenerator
    {
        public override bool HasSet => true;

        public override CollectionGenerator Set(string[] indices, Action<CollectionGenerator> value)
        {
            var builder = Builder.Append(InternalName).String.Append('[');

            for (var index = 0; index < indices.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(indices[index]);
            }
            
            builder.Append("] = ");

            value(this);

            Builder.AppendLine(";");

            return this;
        }
    }

    partial class ArrayGenerator
    {
        public override CollectionGenerator ToArray(string? preFix = null, string? postFix = null)
        {
            Builder.Append(preFix).String.Append(InternalName).Append(postFix);

            return this;
        }
    }
}
