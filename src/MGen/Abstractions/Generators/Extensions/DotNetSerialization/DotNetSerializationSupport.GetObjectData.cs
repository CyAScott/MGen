using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions.DotNetSerialization;

partial class DotNetSerializationCodeGenerator : IHandleMethodCodeGeneration
{
    public void Handle(MethodCodeGenerationArgs args)
    {
        if (args.Builder.MethodSymbol is { Name: "GetObjectData" } &&
            args.Builder.MethodSymbol.ContainingType.ContainingAssembly.Name is "System.Private.CoreLib" or "System.Runtime" &&
            args.Builder.MethodSymbol.ContainingType.ContainingNamespace.Name == "Serialization" &&
            args.Builder.MethodSymbol.ContainingType.Name == DotNetSerializationSupport.InterfaceName &&
            args.Builder.Parent is IHaveProperties parent)
        {
            args.Builder.ExplicitDeclaration.IsExplicitDeclarationEnabled = true;
            args.Handled = true;
            GetObjectData(args, parent);
        }
    }

    void GetObjectData(MethodCodeGenerationArgs args, IHaveProperties parent)
    {
        args.Builder.AddLine("if (info == null) throw new System.ArgumentNullException(\"info\")");
        args.Builder.AddEmptyLine();

        foreach (var property in parent.OfType<PropertyBuilder>())
        {
            if (!property.Enabled ||
                property.ExplicitDeclaration.IsExplicitDeclarationEnabled ||
                property.ReturnType is not CodeType codeType)
            {
                continue;
            }

            var type = codeType.Type;
            if (!type.IsSerializable())
            {
                args.Context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "MG_DotNetSerialization_0000",
                        "Serialization Issue",
                        "Some values will not be serialized.",
                        "SerializationIssue",
                        DiagnosticSeverity.Warning,
                        true), property.PropertySymbols.FirstOrDefault()?.Locations.First() ?? Location.None));
                continue;
            }

            var name = property.Field?.Name ?? property.Name;

            if (type.IsValueType || type.SpecialType == SpecialType.System_String)
            {
                args.Builder.AddLine(new Code(sb => sb
                    .Append("info.AddValue(\"").Append(property.Name).Append("\", ")
                    .Append(name).Append(", ")
                    .Append("typeof(").AppendType(type).Append("))")));
            }
            else
            {
                args.Builder.AddLine(new Code(sb => sb
                    .Append("info.AddValue(\"").Append(property.Name).Append("\", ")
                    .Append(name).Append(", ")
                    .Append(name).Append("?.GetType() ?? typeof(").AppendType(type).Append("))")));
            }

            property.State[DotNetSerializationSupport.InterfaceName] = true;
        }
    }
}