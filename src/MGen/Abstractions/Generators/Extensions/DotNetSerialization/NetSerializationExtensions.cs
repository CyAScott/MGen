using Microsoft.CodeAnalysis;

namespace MGen.Abstractions.Generators.Extensions.DotNetSerialization;

static class NetSerializationExtensions
{
    public static bool IsSerializable(this ITypeSymbol type)
    {
        if (type.IsValueType ||
            type.SpecialType is SpecialType.System_String or SpecialType.System_Array)
        {
            return true;
        }

        if (type is IArrayTypeSymbol arrayType)
        {
            return arrayType.ElementType.IsSerializable();
        }

        var interfaces = type.AllInterfaces;

        foreach (var @interface in interfaces)
        {
            if (@interface.ContainingAssembly.Name is "System.Private.CoreLib" or "System.Runtime" &&
                @interface.ContainingNamespace.Name == "Serialization" &&
                @interface.Name == DotNetSerializationSupport.InterfaceName)
            {
                return true;
            }
        }

        foreach (var attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass?.ContainingAssembly.Name is "System.Private.CoreLib" or "System.Runtime" &&
                attribute.AttributeClass.ContainingNamespace.Name == "System" &&
                attribute.AttributeClass.Name == "SerializableAttribute")
            {
                return true;
            }
        }

        return false;
    }
}