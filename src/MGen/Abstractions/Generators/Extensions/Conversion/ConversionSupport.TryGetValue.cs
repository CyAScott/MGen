using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.Conversion;

partial class ConversionCodeGenerator
{
    void GenerateTryGetValue(MethodCodeGenerationArgs args)
    {
        if (args.Builder.Parent is not IHaveProperties parent)
        {
            return;
        }

        args.Handled = true;

        var switchCase = args.Builder.AddSwitchCase("name");

        var @default = switchCase.Cases.AddDefault();
        @default.Set("value", Code.Null);
        @default.Return(false);
        @default.BreakAtEnd = false;

        foreach (var property in parent.OfType<PropertyBuilder>())
        {
            if (property.Enabled && !property.ExplicitDeclaration.IsExplicitDeclarationEnabled)
            {
                var name = property.Field?.Name ?? property.Name;

                var @case = switchCase.Cases.Add(new(sb => sb.Append('"').Append(property.Name).Append('"')));
                @case.Set("value", name);
                @case.Return(true);
                @case.BreakAtEnd = false;
            }
        }
    }
}