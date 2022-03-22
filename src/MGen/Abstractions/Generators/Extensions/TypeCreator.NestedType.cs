using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MGen.Abstractions.Builders;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions;

public static class TypeCreatorExtensions
{
    public static bool TryToGetNestedType(this PropertyBuilder property, out IHaveTypes builder)
    {
        if (property.State.TryGetValue(TypeCreator.NestedTypeImplementationKey, out var it) && it is IHaveTypes value)
        {
            builder = value;
            return true;
        }

        builder = default!;
        return false;
    }
}

partial class TypeCreator : IHandleOnInit
{
    public const string NestedTypeImplementationKey = "NestedTypeImplementation";

    bool TryToGetType(PropertyBuilder property, out ITypeSymbol type)
    {
        var propertyType = (property.ReturnType as CodeType)?.Type;
        if (propertyType == null)
        {
            type = null!;
            return false;
        }

        if (propertyType is not { TypeKind: TypeKind.Interface })
        {
            type = null!;
            return false;
        }

        if (propertyType.ToDisplayParts().First().Symbol is INamespaceSymbol @namespace &&
            @namespace.Name == "System")
        {
            type = null!;
            return false;
        }

        //todo: try to get element type from collection

        type = propertyType;
        return true;
    }

    [MGenExtension(Id), DebuggerStepThrough]
    class PropertyDetector : IHandlePropertyCodeGeneration
    {
        readonly TypeCreator _typeCreator;

        public PropertyDetector(TypeCreator typeCreator) => _typeCreator = typeCreator;

        public bool Enabled { get; set; } = true;

        public void Handle(PropertyCodeGenerationArgs args) => _typeCreator.GenerateNestedType(args);
    }

    public void Init(InitArgs args) => args.Context.Add(new PropertyDetector(this));

    readonly Dictionary<string, IHaveMembers> _generatedType = new();

    void GenerateNestedType(PropertyCodeGenerationArgs args)
    {
        var property = args.Builder;

        if (property.Parent.CodeGenerators.CurrentFile == null ||
            !TryToGetType(property, out var type))
        {
            return;
        }

        var name = property.Parent.CodeGenerators.CurrentFile.GenerateAttribute.GetDestinationName(type);

        var fullName = property.Parent.GetFullPath(false) + "." + name;

        if (_generatedType.TryGetValue(fullName, out var typeBuilder))
        {
            property.State[NestedTypeImplementationKey] = typeBuilder;
        }
        else if (property.Parent is ClassBuilder classBuilder)
        {
            var @class = classBuilder.Parent.AddClass(name, type, classBuilder.Modifiers.OriginalModifers);
            _generatedType[fullName] = @class;
            property.State[NestedTypeImplementationKey] = @class;
            @class.GenerateCode();
        }
        else if (property.Parent is RecordBuilder recordBuilder)
        {
            var record = recordBuilder.Parent.AddRecord(name, type, recordBuilder.Modifiers.OriginalModifers);
            _generatedType[fullName] = record;
            property.State[NestedTypeImplementationKey] = record;
            record.GenerateCode();
        }
        else if (property.Parent is StructBuilder structBuilder)
        {
            var @struct = structBuilder.Parent.AddStruct(name, type, structBuilder.Modifiers.OriginalModifers);
            _generatedType[fullName] = @struct;
            property.State[NestedTypeImplementationKey] = @struct;
            @struct.GenerateCode();
        }
    }
}