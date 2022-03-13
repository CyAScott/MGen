using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Attributes;

[DebuggerStepThrough]
public sealed class GenerateAttributeRuntime
{
    /// <summary>
    /// If true then the interface types for the properties in this interface will also get classes generated.
    /// </summary>
    public bool GenerateNestedClasses { get; set; } = true;

    /// <summary>
    /// The pattern for creating the name for the class.
    /// </summary>
    public string DestinationNamePattern { get; set; } = "{{interfaceName | name}}Model";

    /// <summary>
    /// The <see cref="System.Text.RegularExpressions.Regex"/> pattern for getting values from the interface name.
    /// </summary>
    public string SourceNamePattern { get; set; } = @"^(I(?<interfaceName>[A-Z]\w+)|(?<name>\w+))$";

    public string GetDestinationName(ITypeSymbol symbol)
    {
        var match = Regex.Match(symbol.Name, SourceNamePattern, RegexOptions.Compiled);

        return Regex.Replace(DestinationNamePattern, @"\{\{\s*(?<parts>\w+\s*(\|\s*\w+\s*)*)\}\}", binding =>
        {
            foreach (var part in binding.Groups["parts"].Value.Split('|'))
            {
                var group = match.Groups[part.Trim()];
                if (group is { Success: true })
                {
                    return group.Value;
                }
            }
            return string.Empty;
        });
    }

    internal static bool TryCreateInstance(AttributeData attribute, out GenerateAttributeRuntime generateAttribute)
    {
        if (attribute.AttributeClass?.ContainingAssembly.Name != "MGen.Abstractions" ||
            attribute.AttributeClass?.ContainingNamespace.ToString() != "MGen" ||
            attribute.AttributeClass?.Name != "GenerateAttribute")
        {
            generateAttribute = default!;
            return false;
        }

        generateAttribute = new GenerateAttributeRuntime();

        foreach (var argument in attribute.NamedArguments)
        {
            switch (argument.Key)
            {
                case nameof(DestinationNamePattern):
                    generateAttribute.DestinationNamePattern = (string)(argument.Value.Value ?? throw new ArgumentNullException(nameof(DestinationNamePattern)));
                    break;
                case nameof(GenerateNestedClasses):
                    generateAttribute.GenerateNestedClasses = (bool)(argument.Value.Value ?? throw new ArgumentNullException(nameof(GenerateNestedClasses)));
                    break;
                case nameof(SourceNamePattern):
                    generateAttribute.SourceNamePattern = (string)(argument.Value.Value ?? throw new ArgumentNullException(nameof(SourceNamePattern)));
                    break;
            }
        }

        return true;
    }
}