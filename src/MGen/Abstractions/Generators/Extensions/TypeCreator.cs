using System.Linq;
using MGen.Abstractions.Builders;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MGen.Abstractions.Generators.Extensions;

/// <summary>
/// Handles generating code for class declaration.
/// </summary>
[MGenExtension(Id)]
public partial class TypeCreator : IHandleOnFileCreated
{
    public const string Id = "MGen." + nameof(TypeCreator);

    public void FileCreated(FileCreatedArgs args)
    {
        var builder = args.Generator.Builder;
        var candidate = args.Generator.Candidate;
        var generator = args.Generator;

        var declaredType = candidate.Types[candidate.Types.Count - 1];
        var name = generator.GenerateAttribute.GetDestinationName(generator.Type);

        if (!TryToDeclareType(candidate, generator.Type, declaredType.Modifiers, builder, name))
        {
            args.Context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "MG_Type_0000",
                    "Unable to generate type",
                    "Unable to generate type for: {0}",
                    "CompileError",
                    DiagnosticSeverity.Error,
                    true), generator.Type.Locations.First(), generator.Type.ToCsString()));
        }
    }

    bool TryToDeclareType(Candidate candidate, ITypeSymbol symbol, SyntaxTokenList modifiers, IHaveTypes builder, string name, int index = 0)
    {
        if (index == candidate.Types.Count - 1)
        {
            //todo: struct

            //todo: record

            var @class = builder.AddClass(name, symbol, modifiers);
            _generatedType[@class.GetFullPath()] = @class;
            @class.GenerateCode();

            return true;
        }

        var type = candidate.Types[index];

        var parentName = type.Identifier.Text.TrimEnd();

        return type switch
        {
            ClassDeclarationSyntax => TryToDeclareType(candidate, symbol, modifiers, type, builder.AddClass(parentName), name, index + 1),
            InterfaceDeclarationSyntax => TryToDeclareType(candidate, symbol, modifiers, type, builder.AddInterface(parentName), name, index + 1),
            RecordDeclarationSyntax => TryToDeclareType(candidate, symbol, modifiers, type, builder.AddRecord(parentName), name, index + 1),
            StructDeclarationSyntax => TryToDeclareType(candidate, symbol, modifiers, type, builder.AddStruct(parentName), name, index + 1),
            _ => false
        };
    }

    bool TryToDeclareType<T>(Candidate candidate, ITypeSymbol symbol, SyntaxTokenList modifiers, TypeDeclarationSyntax type, T builder, string name, int index = 0)
        where T : IHaveGenericParameters, IHaveModifiers, IHaveTypes
    {
        builder.Modifiers.IsPartial = true;

        if (type.TypeParameterList != null)
        {
            foreach (var parameter in type.TypeParameterList.Parameters)
            {
                builder.GenericParameters.Add(parameter.Identifier.Text.TrimEnd());
            }
        }

        return TryToDeclareType(candidate, symbol, modifiers, builder, name, index);
    }
}
