using MGen.Collections;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace MGen.Builder.BuilderContext
{
    public class ClassBuilderContext
    {
        internal ClassBuilderContext(ClassBuilderContext context)
        {
            Builder = context.Builder;
            ClassAttributes = context.ClassAttributes;
            ClassName = context.ClassName;
            CollectionGenerators = context.CollectionGenerators;
            GenerateAttribute = context.GenerateAttribute;
            GeneratorExecutionContext = context.GeneratorExecutionContext;
            Interface = context.Interface;
            Modifiers = context.Modifiers;
            Namespace = context.Namespace;
        }

        internal ClassBuilderContext(GenerateAttributeRuntime generateAttribute,
            SyntaxTokenList modifiers,
            ITypeSymbol @interface,
            string @namespace,
            string className,
            IClassBuilder builder,
            GeneratorExecutionContext generatorExecutionContext,
            CollectionGenerators collectionGenerators)
        {
            Builder = builder;
            ClassName = className;
            CollectionGenerators = collectionGenerators;
            GenerateAttribute = generateAttribute;
            GeneratorExecutionContext = generatorExecutionContext;
            Interface = @interface;
            Modifiers = modifiers;
            Namespace = @namespace;
        }

        /// <summary>
        /// Generators for generating code for collections.
        /// </summary>
        public CollectionGenerators CollectionGenerators { get; }

        /// <summary>
        /// Context passed to a source generator when Microsoft.CodeAnalysis.ISourceGenerator.Execute(Microsoft.CodeAnalysis.GeneratorExecutionContext) is called
        /// </summary>
        public GeneratorExecutionContext GeneratorExecutionContext { get; }

        /// <summary>
        /// The generate attribute for creating this class.
        /// </summary>
        public GenerateAttributeRuntime GenerateAttribute { get; }

        /// <summary>
        /// The builder for building the class that implements <see cref="Interface"/>.
        /// </summary>
        public IClassBuilder Builder { get; }

        /// <summary>
        /// The interface that is being implemented.
        /// </summary>
        public ITypeSymbol Interface { get; }

        /// <summary>
        /// The list of attributes to add to the generated class.
        /// </summary>
        public List<string> ClassAttributes { get; } = new();

        /// <summary>
        /// The original declaration of the interface.
        /// </summary>
        public SyntaxTokenList Modifiers { get; }

        /// <summary>
        /// The name of the class that is being written.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The namespace the for the class that is being written.
        /// </summary>
        public string Namespace { get; }
    }

    public interface IHandleBuildingClasses : IAmAnExtension
    {
        public void Handle(ClassBuilderContext context, Action next);
    }
}
