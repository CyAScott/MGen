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
public class MembersWithCodeDeclaration : IHandleOnFileCreated
{
    public const string Id = "MGen." + nameof(MembersWithCodeDeclaration);

    public void FileCreated(FileCreatedArgs args)
    {
        var builder = args.Generator.Builder;
        var candidate = args.Generator.Candidate;
        var generator = args.Generator;

        var declaredType = candidate.Types[candidate.Types.Count - 1];
        var name = generator.GenerateAttribute.GetDestinationName(generator.Type);

        var type = DeclareType(candidate, generator.Type, declaredType.Modifiers, builder, name);
        if (type == null)
        {
            args.Context.GeneratorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "MG_Class_0001",
                    "Unable to generate class",
                    "Unable to generate class for: {0}",
                    "CompileError",
                    DiagnosticSeverity.Error,
                    true), generator.Type.Locations.First(), generator.Type.ToCsString()));
        }
        else
        {
            args.CreateType(type);
        }
    }

    IHaveMembers? DeclareType<T>(Candidate candidate, ITypeSymbol symbol, SyntaxTokenList modifiers, T builder, string name, int index = 0)
        where T : IHaveClasses, IHaveInterfaces, IHaveRecords, IHaveStructs
    {
        if (index == candidate.Types.Count - 1)
        {
            //todo: struct

            //todo: record

            return builder.AddClass(name, symbol, modifiers);
        }

        var type = candidate.Types[index];

        var parentName = type.Identifier.Text.TrimEnd();

        if (type is ClassDeclarationSyntax)
        {
            return DeclareType(candidate, symbol, modifiers, type, builder.AddClass(parentName), name, index + 1);
        }

        if (type is InterfaceDeclarationSyntax)
        {
            return DeclareType(candidate, symbol, modifiers, type, builder.AddInterface(parentName), name, index + 1);
        }

        if (type is RecordDeclarationSyntax)
        {
            return DeclareType(candidate, symbol, modifiers, type, builder.AddRecord(parentName), name, index + 1);
        }

        if (type is StructDeclarationSyntax)
        {
            return DeclareType(candidate, symbol, modifiers, type, builder.AddStruct(parentName), name, index + 1);
        }

        return null;
    }

    IHaveMembers? DeclareType<T>(Candidate candidate, ITypeSymbol symbol, SyntaxTokenList modifiers, TypeDeclarationSyntax type, T builder, string name, int index = 0)
        where T : IHaveClasses, IHaveGenericParameters, IHaveInterfaces, IHaveModifiers, IHaveRecords, IHaveStructs
    {
        builder.Modifiers.IsPartial = true;

        if (type.TypeParameterList != null)
        {
            foreach (var parameter in type.TypeParameterList.Parameters)
            {
                builder.GenericParameters.Add(parameter.Identifier.Text.TrimEnd());
            }
        }

        return DeclareType(candidate, symbol, modifiers, builder, name, index);
    }
}
