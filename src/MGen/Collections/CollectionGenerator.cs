using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;

namespace MGen.Collections
{
    public abstract partial class CollectionGenerator : ClassBuilderContext
    {
        protected CollectionGenerator(ClassBuilderContext context, ITypeSymbol type, ITypeSymbol implementation, string variableName)
            : base(context)
        {
            Implementation = implementation;
            Type = type;
            VariableName = variableName;

            if (type is IArrayTypeSymbol arrayTypeSymbol)
            {
                TypeArguments = new ImmutableArray<ITypeSymbol>();
                _rank = arrayTypeSymbol.Rank;
                KeyType = null;
                ValueType = arrayTypeSymbol.ElementType;
            }
            else if (type is INamedTypeSymbol namedTypeSymbol)
            {
                TypeArguments = namedTypeSymbol.TypeArguments;
                switch (namedTypeSymbol.TypeArguments.Length)
                {
                    case 0:
                        KeyType = null;
                        ValueType = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("System.Object");
                        break;
                    case 1:
                        KeyType = null;
                        ValueType = namedTypeSymbol.TypeArguments[0];
                        break;
                    case 2:
                        KeyType = namedTypeSymbol.TypeArguments[0];
                        ValueType = namedTypeSymbol.TypeArguments[1];
                        break;
                }
            }
            else
            {
                TypeArguments = new ImmutableArray<ITypeSymbol>();
            }
        }

        /// <summary>
        /// Generics arguments for <see cref="Type"/>.
        /// </summary>
        public ImmutableArray<ITypeSymbol> TypeArguments { get; }

        /// <summary>
        /// The implementation of <see cref="Type"/>.
        /// If <see cref="Type"/> is IDictionary&lt;string, int&gt; then the implementation would Dictionary&lt;string, int&gt;
        /// </summary>
        public virtual ITypeSymbol Implementation { get; }

        /// <summary>
        /// The type for the key for the collection.
        /// </summary>
        public virtual ITypeSymbol? KeyType { get; }

        /// <summary>
        /// The collection type.
        /// This could be an interface like IDictionary&lt;string, int&gt;.
        /// </summary>
        public virtual ITypeSymbol Type { get; }

        /// <summary>
        /// The type for the value for the collection.
        /// </summary>
        public virtual ITypeSymbol? ValueType { get; }

        /// <summary>
        /// The name of the variable for accessing this collection instance.
        /// </summary>
        public virtual string VariableName { get; }

        /// <summary>
        /// The name to used for performing operations on the structure.
        /// </summary>
        protected internal virtual string InternalName => VariableName;
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// Generates code that creates an instance of <see cref="Implementation"/>.
        /// </summary>
        /// <param name="source">
        /// The original copy of this type.
        /// This is useful for copying lengths and comparers.
        /// </param>
        public abstract CollectionGenerator Create(CollectionGenerator? source = null);
    }

    partial class CollectionGenerator
    {
        public CollectionGenerator Enumerate(int variablePostFix, Action<string, string[]> body) =>
            Enumerate(variablePostFix, (_, value, indices) => body(value, indices));

        public CollectionGenerator Enumerate(int variablePostFix, Action<string[]> body) =>
            Enumerate(variablePostFix, (_, _, indices) => body(indices));

        /// <summary>
        /// Generates a loop for enumerating over the structure.
        /// </summary>
        /// <param name="variablePostFix">
        /// When generating index variable names this value will be appended to the end of those names.
        /// </param>
        /// <param name="body">
        /// The method for generating the body of the loop.
        /// </param>
        public abstract CollectionGenerator Enumerate(int variablePostFix, EnumerateBody body);
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure has an add method.
        /// </summary>
        public virtual bool HasAdd => false;

        /// <summary>
        /// Generates code of adding a value to this structure.
        /// </summary>
        public virtual CollectionGenerator Add(Action<CollectionGenerator> value)
        {
            if (!HasAdd || HasKeys)
            {
                throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates code of adding a value to this structure.
        /// The value is the variable name to add to the structure.
        /// </summary>
        public CollectionGenerator Add(string value) =>
            Add(_ => Builder.Append(value));


        /// <summary>
        /// Attempts to upsert a value into this structure.
        /// If the structure supports keys then the value will be set using a key,
        /// else if the structure supports adding then the value will be added to the structure,
        /// else if the structure supports setting by indices then the value will be set using indices.
        /// </summary>
        public void Upsert(string[] indices, string value)
        {
            if (HasKeys)
            {
                Add(indices[0], value);
            }
            else if (HasAdd)
            {
                Add(value);
            }
            else
            {
                Set(indices, value);
            }
        }
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure has a comparer property.
        /// </summary>
        public virtual bool HasComparer => false;

        /// <summary>
        /// Generates code getting the comparer property.
        /// </summary>
        public virtual string GetComparer()
        {
            if (!HasComparer)
            {
                throw new NotSupportedException();
            }

            return InternalName + ".Comparer";
        }
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure has a get method.
        /// </summary>
        public virtual bool HasGet => false;

        /// <summary>
        /// Generates code of getting a value from this structure.
        /// </summary>
        public virtual CollectionGenerator Get(string[] indices, string? preFix = null, string? postFix = null)
        {
            if (!HasGet)
            {
                throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure has a key like a dictionary does.
        /// </summary>
        public virtual bool HasKeys => false;

        /// <summary>
        /// Generates code of adding a key and value to this structure.
        /// </summary>
        public virtual CollectionGenerator Add(Action<CollectionGenerator> key, Action<CollectionGenerator> value)
        {
            if (!HasAdd || !HasKeys)
            {
                throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates code of adding a key and value to this structure.
        /// </summary>
        public virtual CollectionGenerator Add(string key, string value) =>
            Add(_ => Builder.Append(key), _ => Builder.Append(value));
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure has a length value.
        /// </summary>
        public virtual bool HasLength => false;

        /// <summary>
        /// The number of lengths in the array.
        /// </summary>
        public virtual int Rank => HasLength ? _rank : 0;
        readonly int _rank;

        /// <summary>
        /// Generates code for getting the length by rank index.
        /// </summary>
        public virtual string Length(int dimension = 0)
        {
            if (!HasLength)
            {
                throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure has a set method.
        /// </summary>
        public virtual bool HasSet => false;

        /// <summary>
        /// Generates code of setting a value in this structure.
        /// </summary>
        public virtual CollectionGenerator Set(string[] indices, Action<CollectionGenerator> value)
        {
            if (!HasSet)
            {
                throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates code of setting a value in this structure.
        /// The value is the variable name to set the value to.
        /// </summary>
        public CollectionGenerator Set(string[] indices, string value) =>
            Set(indices, _ => Builder.Append(value));
    }

    partial class CollectionGenerator
    {
        /// <summary>
        /// True if the structure supports converting to an array.
        /// </summary>
        public virtual bool HasToArray => true;

        /// <summary>
        /// Converts the collection object to an array.
        /// </summary>
        public virtual CollectionGenerator ToArray(string? preFix = null, string? postFix = null)
        {
            Builder.Append(preFix).String.Append("System.Linq.Enumerable.ToArray(").Append(InternalName).Append(")").Append(postFix);

            return this;
        }
    }
}
