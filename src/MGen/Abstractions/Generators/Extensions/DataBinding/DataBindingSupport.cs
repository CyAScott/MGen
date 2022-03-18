using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.DataBinding;

/// <summary>
/// Implements <see cref="INotifyPropertyChanged"/> and / or <see cref="INotifyPropertyChanging"/> for classes that require it.
/// </summary>
[MGenExtension(Id, after: new[] { MembersWithCodeDeclaration.Id }, before: new [] { MemberDeclaration.Id }), DebuggerStepThrough]
public class DataBindingSupport : IHandleOnInit, IHandleOnTypeGenerated
{
    public const string Id = "MGen." + nameof(DataBindingSupport);

    public void Init(InitArgs args)
    {
        args.Context.Add(new PropertyChangedCodeGenerator());
        args.Context.Add(new PropertyChangingCodeGenerator());
    }

    public void TypeGenerated(TypeGeneratedArgs args)
    {
        if (args.Generator.TryToGetBuilder(out var builder) && builder is IHaveInheritance item)
        {
            foreach (var code in item.Inheritance.OfType<CodeWithInheritedTypeSymbol>())
            {
                foreach (var @interface in code.InheritedTypeSymbol.AllInterfaces)
                {
                    if (@interface.ContainingAssembly.Name == "System.ObjectModel" &&
                        @interface.ContainingNamespace.Name == "ComponentModel")
                    {
                        switch (@interface.Name)
                        {
                            case nameof(INotifyPropertyChanged):
                                args.Generator.State[nameof(INotifyPropertyChanged)] = true;
                                break;
                            case nameof(INotifyPropertyChanging):
                                args.Generator.State[nameof(INotifyPropertyChanging)] = true;
                                break;
                        }
                    }
                }
            }
        }
    }
}

[MGenExtension(Id, after: new [] { DefaultCodeGenerator.Id }), DebuggerStepThrough]
public class PropertyChangedCodeGenerator : IHandlePropertySetCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(PropertyChangedCodeGenerator);

    public void Handle(PropertySetCodeGenerationArgs args)
    {
        if (args.Generator.State.ContainsKey(nameof(INotifyPropertyChanged)))
        {
            args.Builder.Set
                .AddLine(new(sb => sb
                    .Append("PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(\"").Append(args.Builder.Name).Append("\"))")));
        }
    }
}

[MGenExtension(Id, before: new [] { DefaultCodeGenerator.Id }), DebuggerStepThrough]
public class PropertyChangingCodeGenerator : IHandlePropertySetCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(PropertyChangingCodeGenerator);

    public void Handle(PropertySetCodeGenerationArgs args)
    {
        if (args.Generator.State.ContainsKey(nameof(INotifyPropertyChanging)))
        {
            args.Builder.Set
                .AddLine(new(sb => sb
                    .Append("PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"").Append(args.Builder.Name).Append("\"))")));
        }
    }
}