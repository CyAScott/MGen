using MGen.Builder;
using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class ArrayListDetector : CollectionTypeDetector
    {
        public ArrayListDetector(GeneratorExecutionContext context) =>
            ArrayList = context.Compilation.GetTypeByMetadataName("System.Collections.ArrayList") ?? throw new InvalidOperationException("Unable to resolve System.Collections.ArrayList");

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ArrayListGenerator(context, type, ArrayList, variableName);

        public ITypeSymbol ArrayList { get; }

        public override string[] Types => new[]
        {
            "System.Collections.ArrayList",
            "System.Collections.ICollection",
            "System.Collections.IEnumerable",
            "System.Collections.IList"
        };
    }

    partial class ArrayListGenerator : CollectionGenerator
    {
        public ArrayListGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context, type, implementation, variableName)
        {
            HasToArray = type.Name == "ArrayList";

            foreach (var @interface in type.AllInterfaces)
            {
                if (@interface.Name == "ICollection")
                {
                    HasLength = true;
                }
                else if (@interface.Name == "IList")
                {
                    HasAdd = true;
                    HasGet = true;
                    HasSet = true;
                }
            }
        }
    }

    partial class ArrayListGenerator
    {
        public override CollectionGenerator Create(CollectionGenerator? source = null)
        {
            var builder = Builder.Append("var ").String.Append(InternalName)
                .Append(" = new ")
                .Append(Implementation.ContainingNamespace).Append('.').Append(Implementation.Name);

            if (TypeArguments.Length > 0)
            {
                builder.Append('<').AppendType(TypeArguments[0]).Append(">(");
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

    partial class ArrayListGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            if (HasLength && HasGet)
            {
                Builder.AppendLine(builder => builder.Append("for (int _0_").Append(variablePostFix).Append(" = 0; _0_").Append(variablePostFix).Append(" < ").Append(InternalName).Append(".Count; _0_").Append(variablePostFix).Append("++)"));
                Builder.OpenBrace();

                body(this, InternalName + "[_0_" + variablePostFix + "]", "_0_" + variablePostFix);

                Builder.CloseBrace();
            }
            else
            {
                var counterValue = "_0_" + variablePostFix;
                var indexValue = "_1_" + variablePostFix;
                var elementValue = "_2_" + variablePostFix;

                Builder
                    .AppendLine(builder => builder.Append("int ").Append(counterValue).Append(" = 0;"))
                    .AppendLine(builder => builder.Append("foreach (var ").Append(elementValue).Append(" in ").Append(InternalName).Append(')'))
                    .OpenBrace()
                    .AppendLine(builder => builder.Append("int ").Append(indexValue).Append(" = ").Append(counterValue).Append("++;"));

                body(this, elementValue, indexValue);

                Builder.CloseBrace();
            }

            return this;
        }
    }

    partial class ArrayListGenerator
    {
        public override bool HasAdd { get; }

        public override CollectionGenerator Add(Action<CollectionGenerator> value)
        {
            if (!HasAdd)
            {
                return base.Add(value);
            }

            Builder.Append(InternalName).String.Append(".Add(");
            value(this);
            Builder.AppendLine(");");

            return this;
        }
    }

    partial class ArrayListGenerator
    {
        public override bool HasGet { get; }

        public override CollectionGenerator Get(string[] indices, string? preFix = null, string? postFix = null)
        {
            if (!HasGet)
            {
                return base.Get(indices, preFix, postFix);
            }

            Builder.Append(preFix).String.Append(InternalName).Append('[').Append(indices[0]).Append(']').Append(postFix);

            return this;
        }
    }

    partial class ArrayListGenerator
    {
        public override bool HasLength { get; }

        public override string Length(int dimension = 0) => HasLength ? InternalName + ".Count" : base.Length(dimension);
    }

    partial class ArrayListGenerator
    {
        public override bool HasSet { get; }

        public override CollectionGenerator Set(string[] indices, Action<CollectionGenerator> value)
        {
            if (!HasSet)
            {
                return base.Set(indices, value);
            }

            Builder.Append(InternalName).String.Append('[').Append(indices[0]).Append("]  = ");
            value(this);
            Builder.AppendLine(";");

            return this;
        }
    }

    partial class ArrayListGenerator
    {
        public override bool HasToArray { get; }

        public override CollectionGenerator ToArray(string? preFix = null, string? postFix = null)
        {
            var builder = Builder.Append(preFix).String;

            if (HasToArray)
            {
                builder.Append(InternalName).Append(".ToArray()");
            }
            else
            {
                builder.Append("System.Linq.Enumerable.ToArray(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Cast<object>(").Append(InternalName).Append("))");
            }

            builder.Append(postFix);

            return this;
        }
    }
}
