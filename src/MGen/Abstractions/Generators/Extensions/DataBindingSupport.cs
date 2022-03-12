using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions;

/// <summary>
/// Implements <see cref="INotifyPropertyChanged"/> and / or <see cref="INotifyPropertyChanging"/> for classes that require it.
/// </summary>
[MGenExtension(Id, after: new[] { MemberDeclaration.Id }), DebuggerStepThrough]
public class DataBindingSupport : IHandleOnInit
{
    internal static bool HasType(TypeGenerator generator, string name)
    {
        if (generator.State.TryGetValue(name, out var hasType))
        {
            return (bool)hasType;
        }

        if (!generator.State.TryGetValue(MembersWithCodeDeclaration.MembersWithCodeDeclarationKey, out var value) ||
            value is not IHaveInheritance item)
        {
            generator.State[name] = false;
            return false;
        }

        generator.State[name] = false;

        foreach (var code in item.Inheritance.OfType<CodeWithInheritedTypeSymbol>())
        {
            if (code.InheritedTypeSymbol is INamedTypeSymbol symbol)
            {
                foreach (var @interface in symbol.AllInterfaces)
                {
                    if (@interface.ContainingAssembly.Name == "System.ObjectModel" &&
                        @interface.ContainingNamespace.Name == "ComponentModel")
                    {
                        switch (@interface.Name)
                        {
                            case nameof(INotifyPropertyChanged):
                                generator.State[nameof(INotifyPropertyChanged)] = true;
                                break;
                            case nameof(INotifyPropertyChanging):
                                generator.State[nameof(INotifyPropertyChanging)] = true;
                                break;
                        }
                    }
                }
            }
        }

        return (bool)generator.State[name];
    }

    public const string Id = "MGen." + nameof(DataBindingSupport);

    public void Init(InitArgs args)
    {
        args.Context.Add(new PropertyChangedCodeGenerator());
        args.Context.Add(new PropertyChangingCodeGenerator());
    }
}

[MGenExtension(Id, after: new[] { DefaultCodeGenerator.Id }), DebuggerStepThrough]
public class PropertyChangedCodeGenerator : IHandlePropertySetCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(PropertyChangedCodeGenerator);

    public void Handle(PropertySetCodeGenerationArgs args)
    {
        if (DataBindingSupport.HasType(args.Generator, nameof(INotifyPropertyChanged)))
        {
            args.Builder.Set.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 2)
                .Append("PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(\"").Append(args.Builder.Name).AppendLine("\"));")));
        }
    }
}

[MGenExtension(Id, before: new[] { DefaultCodeGenerator.Id }), DebuggerStepThrough]
public class PropertyChangingCodeGenerator : IHandlePropertySetCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(PropertyChangingCodeGenerator);

    public void Handle(PropertySetCodeGenerationArgs args)
    {
        if (DataBindingSupport.HasType(args.Generator, nameof(INotifyPropertyChanging)))
        {
            args.Builder.Set.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 2)
                .Append("PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"").Append(args.Builder.Name).AppendLine("\"));")));
            args.Builder.Set.Add(Code.Empty);
        }
    }
}