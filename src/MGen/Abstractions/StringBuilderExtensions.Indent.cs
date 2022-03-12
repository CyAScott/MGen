using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions;

static partial class StringBuilderExtensions
{
    [DebuggerStepThrough]
    public static StringBuilder AppendIndent(this StringBuilder stringBuilder, int indent)
    {
        if (indent > 0)
        {
            stringBuilder.Append(' ', 4 * indent);
        }
        return stringBuilder;
    }
}