using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Attributes;

static class Extensions
{
    [DebuggerStepThrough]
    public static List<object> GetMGenAttributes(this ITypeSymbol symbol)
    {
        var attributes = symbol.GetAttributes();
        var list = new List<object>(attributes.Length);

        foreach (var attribute in attributes)
        {
            if (GenerateAttributeRuntime.TryCreateInstance(attribute, out var generateAttribute))
            {
                list.Add(generateAttribute);
            }
        }

        return list;
    }
}
