using System;
using System.Diagnostics;
using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions;

/// <summary>
/// Implements MGen.ISupportConversion for classes that require it.
/// </summary>
[MGenExtension(Id, after: new[] { MemberDeclaration.Id }), DebuggerStepThrough]
public class ConversionSupport : IHandleOnInit, IHandleOnTypeGenerated
{
    bool SupportsConversion(ITypeSymbol typeSymbol) =>
        typeSymbol.ContainingAssembly.Name == "MGen.Abstractions" &&
        typeSymbol.ContainingNamespace.Name == "MGen" &&
        typeSymbol.Name == InterfaceName ||
        typeSymbol.AllInterfaces.Any(SupportsConversion);

    public const string Id = "MGen." + nameof(ConversionSupport);

    public const string InterfaceName = "ISupportConversion";

    public void Init(InitArgs args) => args.Context.Add(new ConversionCodeGenerator());

    public void TypeGenerated(TypeGeneratedArgs args)
    {
        if (args.Generator.TryToGetBuilderBaseOnInheritance(SupportsConversion, out var builder))
        {
            args.Generator.State[InterfaceName] = true;

            var ctor = builder.AddConstructor();

            ctor.ArgumentParameters.Add("MGen.ISupportConversion", "obj")
                .Attributes.Add("System.Diagnostics.CodeAnalysis.NotNullAttribute");
            ctor.Modifiers.IsProtected = true;
            ctor.State[InterfaceName] = true;

            args.GenerateCode(ctor);
        }
    }
}

[MGenExtension(Id, before: new [] { DefaultCodeGenerator.Id }), DebuggerStepThrough]
public partial class ConversionCodeGenerator : IHandleMethodCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(ConversionCodeGenerator);

    public void Handle(MethodCodeGenerationArgs args)
    {
        if (args.Builder.MethodSymbol != null)
        {
            if (args.Builder.MethodSymbol.Name == "TryGetValue" &&
                args.Builder.MethodSymbol.ContainingType.ContainingAssembly.Name == "MGen.Abstractions" &&
                args.Builder.MethodSymbol.ContainingType.ContainingNamespace.Name == "MGen" &&
                args.Builder.MethodSymbol.ContainingType.Name == ConversionSupport.InterfaceName)
            {
                args.Builder.ExplicitDeclaration.IsExplicitDeclarationEnabled = true;

                GenerateTryGetValue(args);
            }
            else if (args.Builder.MethodSymbol.ContainingType.ContainingAssembly.Name is "System.Runtime" or "System.Private.CoreLib" &&
                args.Builder.MethodSymbol.ContainingType.ContainingNamespace.Name == "System" &&
                args.Builder.MethodSymbol.ContainingType.Name == nameof(IConvertible))
            {
                args.Builder.ExplicitDeclaration.IsExplicitDeclarationEnabled = true;
                args.Handled = true;

                switch (args.Builder.Name)
                {
                    case "GetTypeCode":
                        args.Builder.Return("System.TypeCode.Object");
                        break;
                    case "ToString":
                        args.Builder.Return("ToString()!");
                        break;
                    case "ToType":
                        GenerateToType(args);
                        break;
                    default:
                        args.Builder.AddLine("throw new System.NotSupportedException()");
                        break;
                }
            }
        }
    }
}