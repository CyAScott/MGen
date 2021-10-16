using MGen.Builder;
using MGen.Collections;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MGen
{
    static class ModelBuilder
    {
        public static IEnumerable<(string FullName, string Code)> GenerateCode(this List<InterfaceInfo> interfaces, GeneratorExecutionContext generatorExecutionContext)
        {
            var builder = new ClassBuilder(generatorExecutionContext);
            var collectionGenerators = new CollectionGenerators(generatorExecutionContext);

            foreach (var @interface in interfaces)
            {
                var generateAttribute = @interface.Attributes.OfType<GenerateAttributeRuntime>().Single();

                var context = builder.AppendClass(@interface, generateAttribute, generatorExecutionContext, collectionGenerators);

                var code = builder.ToString();
                var fullName = context.Namespace + "." + context.ClassName;

                builder.Clear();

                yield return (fullName, code);
            }
        }

        public static bool TryCreateGenerateAttribute(AttributeData attribute, out GenerateAttributeRuntime generateAttribute)
        {
            if (attribute.AttributeClass?.ContainingAssembly.Name != "MGen.Abstractions" ||
                attribute.AttributeClass?.ContainingNamespace.ToString() != "MGen" ||
                attribute.AttributeClass?.Name != "GenerateAttribute")
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                generateAttribute = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                return false;
            }

            generateAttribute = new GenerateAttributeRuntime();

            SetProperties(attribute, typeof(GenerateAttributeRuntime), generateAttribute);

            return true;
        }

        public static List<object> GetMGenAttributes(this ITypeSymbol interfaceSymbol)
        {
            var attributes = interfaceSymbol.GetAttributes();
            var list = new List<object>(attributes.Length);

            foreach (var attribute in attributes)
            {
                if (!IsAMGenAttribute(attribute))
                {
                    continue;
                }

                if (TryCreateGenerateAttribute(attribute, out var generateAttribute))
                {
                    list.Add(generateAttribute);
                    continue;
                }

                if (!TryCreateInstance(attribute, out var instance, out var type))
                {
                    continue;
                }

                SetProperties(attribute, type, instance);

                list.Add(instance);
            }

            return list;
        }

        public static bool IsAMGenAttribute(AttributeData attribute)
        {
            var baseType = attribute.AttributeClass;

            while (baseType?.BaseType != null &&
                (
                    baseType.BaseType.ContainingAssembly.Name != "MGen.Abstractions" ||
                    baseType.BaseType.ContainingNamespace.Name != "MGen" ||
                    baseType.BaseType.Name != "MGenAttribute"
                ))
            {
                baseType = baseType.BaseType;
            }

            return baseType?.BaseType != null;
        }

        public static bool TryCreateInstance(AttributeData attribute, out object instance, out Type type)
        {
            var @namespace = attribute.AttributeClass?.ContainingNamespace.ToString() ?? "";
            var name = attribute.AttributeClass?.Name ?? "";

            Assembly assembly;
            try
            {
                assembly = Assembly.Load(attribute.AttributeClass?.ContainingAssembly.Name ?? "");
            }
            catch
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    instance = null;
                    type = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    return false;
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            type = assembly.GetType(@namespace + "." + name, false);
#pragma warning restore CS8601 // Possible null reference assignment.
            if (type == null)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                instance = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                return false;
            }

            if (attribute.ConstructorArguments.Length == 0)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Possible null reference assignment.
                instance = Activator.CreateInstance(type);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                return true;
            }

            foreach (var ctor in type.GetConstructors())
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length != attribute.ConstructorArguments.Length)
                {
                    continue;
                }

                var matchFound = true;

                for (var index = 0; index < attribute.ConstructorArguments.Length; index++)
                {
                    var argument = attribute.ConstructorArguments[index];
                    var parameterType = parameters[index].ParameterType;
                    if (parameterType.Name != argument.Type?.Name)
                    {
                        matchFound = false;
                        break;
                    }
                }

                if (!matchFound)
                {
                    continue;
                }

                var arguments = new object?[attribute.ConstructorArguments.Length];
                for (var index = 0; index < attribute.ConstructorArguments.Length; index++)
                {
                    arguments[index] = attribute.ConstructorArguments[index].Value;
                }

                instance = ctor.Invoke(arguments);

                return true;
            }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            instance = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            return false;
        }

        public static string GetDestinationName(this string namePattern, string sourceNamePattern, ITypeSymbol symbol)
        {
            var match = Regex.Match(symbol.Name, sourceNamePattern, RegexOptions.Compiled);

            return Regex.Replace(namePattern, @"\{\{\s*(?<parts>\w+\s*(\|\s*\w+\s*)*)\}\}", binding =>
            {
                foreach (var part in binding.Groups["parts"].Value.Split('|'))
                {
                    var group = match.Groups[part.Trim()];
                    if (group is {Success: true})
                    {
                        return group.Value;
                    }
                }
                return string.Empty;
            });
        }

        public static void SetProperties(AttributeData attribute, Type type, object instance)
        {
            var arguments = attribute.NamedArguments;

            for (var index = 0; index < arguments.Length; index++)
            {
                var argument = arguments[index];

                var property = type.GetProperty(argument.Key);

                property?.SetValue(instance, argument.Value.Value);
            }
        }
    }
}
