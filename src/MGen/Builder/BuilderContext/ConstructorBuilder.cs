using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace MGen.Builder.BuilderContext
{
    public class ConstructorBuilder : List<ConstructorParameter>
    {
        /// <summary>
        /// The lines for the constructor body.
        /// </summary>
        public List<Action<ConstructorBuilderContext>> Body { get; } = new();

        /// <summary>
        /// The optional XML comments for the class constructor.
        /// </summary>
        public List<string> XmlComments { get; } = new();

        /// <summary>
        /// The constructor modifier (i.e. public, internal, etc.)
        /// </summary>
        public string Modifier { get; set; } = "public";

        /// <summary>
        /// Resets the constructor.
        /// </summary>
        public void Reset()
        {
            Body.Clear();
            Clear();
            Modifier = "public";
            XmlComments.Clear();
        }
    }

    public class ConstructorParameter
    {
        public ConstructorParameter(ITypeSymbol type, string name, string fullLine)
        {
            Type = type;
            Name = name;
            FullLine = fullLine;
        }

        /// <summary>
        /// The type of parameter (i.e. System.Int32).
        /// </summary>
        public ITypeSymbol Type { get; }

        /// <inheritdoc />
        public override string ToString() => FullLine;

        /// <summary>
        /// The full line for the parameter (i.e. "[NotNull] string name")
        /// </summary>
        public string FullLine { get; }

        /// <summary>
        /// The name of the parameter (i.e. count, name, etc.).
        /// </summary>
        public string Name { get; }
    }
}
