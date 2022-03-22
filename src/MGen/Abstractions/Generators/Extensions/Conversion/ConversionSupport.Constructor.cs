using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions.Conversion;

partial class ConversionCodeGenerator : IHandleConstructorCodeGeneration
{
    public void Handle(ConstructorCodeGenerationArgs args)
    {
        if (!args.Builder.State.ContainsKey(ConversionSupport.InterfaceName) ||
            args.Builder.Parent is not IHaveProperties parent)
        {
            return;
        }

        args.Handled = true;

        var ctor = args.Builder;

        ctor.AddLine("if (obj == null) throw new System.ArgumentNullException(\"obj\")");
        ctor.AddEmptyLine();
        ctor.AddLine("object? value");
        ctor.AddEmptyLine();

        foreach (var property in parent.OfType<PropertyBuilder>())
        {
            if (!property.Enabled ||
                property.ExplicitDeclaration.IsExplicitDeclarationEnabled ||
                property.ReturnType is not CodeType codeType)
            {
                continue;
            }

            var name = property.Field?.Name ?? property.Name;
            var type = codeType.Type;

            if (type.SpecialType == SpecialType.System_String)
            {
                ConvertStringType(ctor, property, name);
            }
            else if (type.IsValueType)
            {
                ConvertValueType(ctor, property, name, type);
            }
            else
            {
                ConvertRefType(ctor, property, name, type);
            }
        }
    }

    void ConvertRefType(ConstructorBuilder ctor, PropertyBuilder property, string fieldName, ITypeSymbol type)
    {
        ctor.AddLine(new(sb =>
        {
            sb
                .Append(fieldName)
                .Append(" = !obj.TryGetValue(\"").Append(property.Name)
                .Append("\", out value) || value == null ? default : (")
                .AppendType(type).Append(")System.Convert.ChangeType(value, typeof(");
            if (property.TryToGetNestedType(out var nestedType))
            {
                sb.Append(nestedType.GetFullPath());
            }
            else
            {
                sb.AppendType(type);
            }
            sb.Append("))");
        }));
    }

    void ConvertStringType(ConstructorBuilder ctor, PropertyBuilder property, string fieldName) =>
        ctor.AddLine(new(sb => sb
            .Append(fieldName)
            .Append(" = !obj.TryGetValue(\"").Append(property.Name).Append("\", out value) ? default : value as string ?? value?.ToString()")));

    void ConvertValueType(ConstructorBuilder ctor, PropertyBuilder property, string fieldName, ITypeSymbol type) =>
        ctor.AddLine(new(sb => sb
            .Append(fieldName)
            .Append(" = !obj.TryGetValue(\"").Append(property.Name).Append("\", out value) ? default : value as ")
            .AppendType(type).Append("? ?? ")
            .Append("(").AppendType(type).Append(")")
            .Append("System.ComponentModel.TypeDescriptor.GetConverter(")
            .Append("typeof(").AppendType(type).Append(")).ConvertFrom(value)")));
}