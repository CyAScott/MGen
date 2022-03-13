using System;
using System.Diagnostics;
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
public class MembersWithCodeDeclaration : IHandleOnTypeGenerated
{
    public const string MembersWithCodeDeclarationKey = "MGen." + nameof(MembersWithCodeDeclaration);
    public const string Id = "MGen." + nameof(MembersWithCodeDeclaration);

    public void TypeGenerated(TypeGeneratedArgs args)
    {
        var builder = args.Generator.Builder;
        var candidate = args.Generator.Candidate;
        var generator = args.Generator;

        var declaredType = generator.Candidate.Types[generator.Candidate.Types.Count - 1];
        var name = generator.GenerateAttribute.GetDestinationName(generator.Type);

        var @class = DeclareClass(candidate, generator.Type, declaredType.Modifiers, builder, name);
        if (@class == null)
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
            generator.State[MembersWithCodeDeclarationKey] = @class;
        }

        //todo: struct

        //todo: record
    }

    ClassBuilder? DeclareClass<T>(Candidate candidate, ITypeSymbol symbol, SyntaxTokenList modifiers, T builder, string name, int index = 0)
        where T : IHaveClasses, IHaveInterfaces, IHaveRecords, IHaveStructs
    {
        if (index == candidate.Types.Count - 1)
        {
            return builder.AddClass(name, symbol, modifiers);
        }

        var type = candidate.Types[index];

        var parentName = type.Identifier.Text.TrimEnd();

        if (type is ClassDeclarationSyntax)
        {
            return DeclareClass(candidate, symbol, modifiers, type, builder.AddClass(parentName), name, index + 1);
        }

        if (type is InterfaceDeclarationSyntax)
        {
            return DeclareClass(candidate, symbol, modifiers, type, builder.AddInterface(parentName), name, index + 1);
        }

        if (type is RecordDeclarationSyntax)
        {
            return DeclareClass(candidate, symbol, modifiers, type, builder.AddRecord(parentName), name, index + 1);
        }

        if (type is StructDeclarationSyntax)
        {
            return DeclareClass(candidate, symbol, modifiers, type, builder.AddStruct(parentName), name, index + 1);
        }

        return null;
    }

    ClassBuilder? DeclareClass<T>(Candidate candidate, ITypeSymbol symbol, SyntaxTokenList modifiers, TypeDeclarationSyntax type, T builder, string name, int index = 0)
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

        return DeclareClass(candidate, symbol, modifiers, builder, name, index);
    }
}

public static class MembersWithCodeDeclarationExtensions
{
    [DebuggerStepThrough]
    public static bool TryToGetBuilder(this TypeGenerator generator, out IHaveMembersWithCode builder)
    {
        if (!generator.State.TryGetValue(MembersWithCodeDeclaration.MembersWithCodeDeclarationKey, out var value) ||
            value is not IHaveMembersWithCode valueAsBuilder)
        {
            builder = default!;
            return false;
        }

        builder = valueAsBuilder;
        return true;
    }

    [DebuggerStepThrough]
    public static bool TryToGetBuilderBaseOnInheritance(this TypeGenerator generator, Func<ITypeSymbol, bool> predict, out IHaveMembersWithCode builder)
    {
        if (generator.State.TryGetValue(MembersWithCodeDeclaration.MembersWithCodeDeclarationKey, out var value) &&
            value is IHaveInheritance item and IHaveMembersWithCode valueAsBuilder &&
            item.Inheritance
                .OfType<CodeWithInheritedTypeSymbol>()
                .Any(it => predict(it.InheritedTypeSymbol)))
        {
            builder = valueAsBuilder;

            return true;
        }

        builder = default!;
        return false;
    }
}