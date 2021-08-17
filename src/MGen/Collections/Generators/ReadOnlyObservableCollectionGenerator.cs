using MGen.Builder;
using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;

namespace MGen.Collections.Generators
{
    class ReadOnlyObservableCollectionDetector : CollectionTypeDetector
    {
        public ReadOnlyObservableCollectionDetector(GeneratorExecutionContext context)
        {
            List = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.List`1") ?? throw new InvalidOperationException("Unable to resolve System.Collections.Generic.List");
            ObservableCollection = context.Compilation.GetTypeByMetadataName("System.Collections.ObjectModel.ObservableCollection`1") ?? throw new InvalidOperationException("Unable to resolve System.Collections.ObjectModel.ObservableCollection");
        }

        public ITypeSymbol List { get; }
        public ITypeSymbol ObservableCollection { get; }

        public override CollectionGenerator Create(ClassBuilderContext context, ITypeSymbol type, string variableName) =>
            new ReadOnlyObservableCollectionGenerator(context, type, type.Name == "ReadOnlyCollection" ? List : ObservableCollection, variableName);

        public override string[] Types => new[]
        {
            "System.Collections.ObjectModel.ReadOnlyCollection`1",
            "System.Collections.ObjectModel.ReadOnlyObservableCollection`1"
        };
    }

    partial class ReadOnlyObservableCollectionGenerator : ListGenerator
    {
        public ReadOnlyObservableCollectionGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implmentation, string variableName)
            : base(context, type, implmentation,  variableName)
        {
            InternalName = "_" + Guid.NewGuid().ToString().Replace("-", "");
        }

        protected internal override string InternalName { get; }
    }

    partial class ReadOnlyObservableCollectionGenerator
    {
        public override CollectionGenerator Create(CollectionGenerator? source = null)
        {
            base.Create(source);

            Builder.AppendLine(builder => builder
                .Append("var ").Append(VariableName)
                .Append(" = new ")
                .Append(Type.ContainingNamespace).Append('.').Append(Type.Name)
                .Append('<').AppendType(TypeArguments[0]).Append(">(")
                .Append(InternalName)
                .Append(");"));

            return this;
        }
    }

    partial class ReadOnlyObservableCollectionGenerator
    {
        public override CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body)
        {
            Builder.AppendLine(builder => builder.Append("for (int _0_").Append(variablePostFix).Append(" = 0; _0_").Append(variablePostFix).Append(" < ").Append(VariableName).Append(".Count; _0_").Append(variablePostFix).Append("++)"));
            Builder.OpenBrace();

            body(this, VariableName + "[_0_" + variablePostFix + "]", "_0_" + variablePostFix);

            Builder.CloseBrace();

            return this;
        }
    }
}
