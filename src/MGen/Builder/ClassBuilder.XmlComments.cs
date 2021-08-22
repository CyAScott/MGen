using Microsoft.CodeAnalysis;

namespace MGen.Builder
{
    public partial interface IClassBuilder
    {
        /// <summary>
        /// Append the XML comments for a symbol.
        /// </summary>
        IClassBuilder AppendXmlComments(ISymbol symbol);
    }

    partial class ClassBuilder
    {
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
    }
}
