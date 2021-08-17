using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Text;

namespace MGen.Builder
{
    public interface IClassBuilder
    {
        /// <summary>
        /// Appends a constructor to the class.
        /// </summary>
        IClassBuilder AppendConstructor(ClassBuilderContext context, ConstructorBuilder constructor);

        /// <summary>
        /// Appends some text to the class.
        /// If this is the first text for the line, it will be automatically indented.
        /// </summary>
        IClassBuilder Append(Action<StringBuilder> builder);

        /// <summary>
        /// Appends some text to the class.
        /// If this is the first text for the line, it will be automatically indented.
        /// </summary>
        IClassBuilder Append(ITypeSymbol? type);

        /// <summary>
        /// Appends some text to the class.
        /// If this is the first text for the line, it will be automatically indented.
        /// </summary>
        IClassBuilder Append(object? obj = null);

        /// <summary>
        /// If the line is not already indented then an indent will be appended.
        /// </summary>
        IClassBuilder AppendIndent();

        /// <summary>
        /// Appends some text to the class followed by a new line.
        /// If this is the first text for the line, it will be automatically indented.
        /// </summary>
        IClassBuilder AppendLine(Action<StringBuilder> lineBuilder);

        /// <summary>
        /// Appends some text to the class followed by a new line.
        /// If this is the first text for the line, it will be automatically indented.
        /// </summary>
        IClassBuilder AppendLine(ITypeSymbol? type);

        /// <summary>
        /// Appends some text to the class followed by a new line.
        /// If this is the first text for the line, it will be automatically indented.
        /// </summary>
        IClassBuilder AppendLine(object? obj = null);

        /// <summary>
        /// Appends paramters to a method, indexer property, or constructor.
        /// </summary>
        IClassBuilder AppendParameters(ImmutableArray<IParameterSymbol> parameters);

        /// <summary>
        /// Append the XML comments for a symbol.
        /// </summary>
        IClassBuilder AppendXmlComments(ISymbol symbol);

        /// <summary>
        /// Opens a brace for a block of code.
        /// </summary>
        IClassBuilder CloseBrace(int repeat = 1);

        /// <summary>
        /// Closes a brace for a block of code.
        /// </summary>
        IClassBuilder OpenBrace();

        /// <summary>
        /// The string builder for the class.
        /// </summary>
        StringBuilder String { get; }

        /// <summary>
        /// The current number of indents.
        /// </summary>
        int IndentLevel { get; set; }

        /// <summary>
        /// Adds a nested class to implement.
        /// </summary>
        string Append(ClassBuilderContext context, ITypeSymbol @interface);
    }

    /// <summary>
    /// A string builder used for generating the C# code for the class.
    /// </summary>
    partial class ClassBuilder : IClassBuilder
    {
        private bool _lineStarted;

        public IClassBuilder Append(Action<StringBuilder> builder)
        {
            AppendIndent();
            builder(String);
            return this;
        }

        public IClassBuilder Append(ITypeSymbol? type)
        {
            AppendIndent();

            if (type != null)
            {
                String.AppendType(type);
            }

            return this;
        }

        public IClassBuilder Append(object? obj = null)
        {
            AppendIndent();

            if (obj != null)
            {
                String.Append(obj);
            }

            return this;
        }

        public IClassBuilder AppendIndent()
        {
            if (_lineStarted)
            {
                return this;
            }

            _lineStarted = true;

            if (IndentLevel > 0)
            {
                String.Append(' ', 4 * IndentLevel);
            }

            return this;
        }

        public IClassBuilder AppendLine(Action<StringBuilder> lineBuilder)
        {
            AppendIndent();

            lineBuilder(String);

            String.AppendLine();

            _lineStarted = false;

            return this;
        }

        public IClassBuilder AppendLine(ITypeSymbol? type)
        {
            AppendIndent();

            if (type != null)
            {
                String.AppendType(type);
                String.AppendLine();
            }
            else
            {
                String.AppendLine();
            }

            _lineStarted = false;

            return this;
        }

        public IClassBuilder AppendLine(object? obj = null)
        {
            AppendIndent();

            if (obj != null)
            {
                if (obj is string line)
                {
                    String.AppendLine(line);
                }
                else
                {
                    String.Append(obj);
                    String.AppendLine();
                }
            }
            else
            {
                String.AppendLine();
            }

            _lineStarted = false;

            return this;
        }

        public IClassBuilder AppendParameters(ImmutableArray<IParameterSymbol> parameters)
        {
            for (var index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    String.Append(", ");
                }

                Append(parameters[index].Type).String.Append(" ").Append(parameters[index].Name);
            }

            return this;
        }

        public IClassBuilder AppendXmlComments(ISymbol symbol)
        {
            var comments = symbol.GetDocumentationCommentXml();
            if (string.IsNullOrEmpty(comments))
            {
                return this;
            }

            var lines = comments?.Split('\n') ?? new string[0];

            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimStart().TrimEnd('\r');
                if (!string.IsNullOrEmpty(line) &&
                    !rawLine.StartsWith("<member name=") &&
                    !rawLine.StartsWith("</member>"))
                {
                    Append("/// ");
                    AppendLine(line);
                }
            }

            return this;
        }

        public IClassBuilder CloseBrace(int repeat = 1)
        {
            while (repeat > 0)
            {
                IndentLevel--;
                AppendLine("}");
                repeat--;
            }
            return this;
        }

        public IClassBuilder OpenBrace()
        {
            AppendLine("{");
            IndentLevel++;
            return this;
        }

        public StringBuilder String { get; } = new(4096);

        public int IndentLevel { get; set; }

        public override string ToString() => String.ToString();

        public void Clear()
        {
            IndentLevel = 0;
            String.Clear();
        }
    }

    public static class CodeGenertorExtensions
    {
        public static StringBuilder AppendType(this StringBuilder stringBuilder, ITypeSymbol? type) =>
            stringBuilder.Append(type.ToCsString());

        public static string ToCsString(this ITypeSymbol? type) => type?.ToString().Replace("*", "") ?? "";
    }
}
