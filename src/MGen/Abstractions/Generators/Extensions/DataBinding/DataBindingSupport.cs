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
[MGenExtension(Id, before: new [] { MemberDeclaration.Id }), DebuggerStepThrough]
public class DataBindingSupport : IHandleOnInit, IHandleOnTypeCreated
{
    public const string Id = "MGen." + nameof(DataBindingSupport);

    public void Init(InitArgs args)
    {
        args.Context.Add(new PropertyChangedCodeGenerator());
        args.Context.Add(new PropertyChangingCodeGenerator());
    }

    public void TypeCreated(TypeCreatedArgs args)
    {
        if (args.Builder is IHaveState type and IHaveInheritance item)
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
                                type.State[nameof(INotifyPropertyChanged)] = true;
                                break;
                            case nameof(INotifyPropertyChanging):
                                type.State[nameof(INotifyPropertyChanging)] = true;
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
        
        if (args.Builder.Parent is IHaveState type && type.State.ContainsKey(nameof(INotifyPropertyChanged)))
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
        if (args.Builder.Parent is IHaveState type && type.State.ContainsKey(nameof(INotifyPropertyChanging)))
        {
            args.Builder.Set
                .AddLine(new(sb => sb
                    .Append("PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"").Append(args.Builder.Name).Append("\"))")));
        }
    }
}