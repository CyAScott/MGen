using System;
using System.Diagnostics;
using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.Conversion;

/// <summary>
/// Implements MGen.ISupportConversion for classes that require it.
/// </summary>
[MGenExtension(Id, after: new[] { MemberDeclaration.Id }), DebuggerStepThrough]
public class ConversionSupport : IHandleOnInit, IHandleOnTypeCreated
{
    public const string Id = "MGen." + nameof(ConversionSupport);

    public const string InterfaceName = "ISupportConversion";

    public void Init(InitArgs args) => args.Context.Add(new ConversionCodeGenerator());

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveConstructors builder and IHaveMembersWithCode and IHaveInheritance item)
        {
            foreach (var code in item.Inheritance.OfType<CodeWithInheritedTypeSymbol>())
            {
                foreach (var @interface in code.InheritedTypeSymbol.AllInterfaces)
                {
                    if (@interface.ContainingAssembly.Name == "MGen.Abstractions" &&
                        @interface.ContainingNamespace.Name == "MGen" &&
                        @interface.Name == InterfaceName)
                    {
                        var ctor = builder.AddConstructor();

                        ctor.ArgumentParameters.Add("MGen.ISupportConversion", "obj")
                            .Attributes.Add("System.Diagnostics.CodeAnalysis.NotNullAttribute");
                        ctor.Modifiers.IsProtected = true;
                        ctor.State[InterfaceName] = true;

                        ctor.GenerateCode();

                        return;
                    }
                }
            }
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