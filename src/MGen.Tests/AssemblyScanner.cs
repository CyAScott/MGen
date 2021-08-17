using System;
using System.Linq;

namespace MGen
{
    public static class AssemblyScanner
    {
        public static Type FindType(Func<Type, bool> filter) =>
            typeof(AssemblyScanner)
                .Assembly
                .ExportedTypes
                .SingleOrDefault(filter);

        public static Type FindImplementationFor(Type interfaceType) =>
            FindType(type =>
            {
                if (!type.IsClass ||
                    type.IsAbstract ||
                    type.Namespace != interfaceType.Namespace)
                {
                    return false;
                }

                if (!interfaceType.IsGenericTypeDefinition)
                {
                    return !type.IsGenericTypeDefinition && interfaceType.IsAssignableFrom(type);
                }

                if (!type.IsGenericTypeDefinition)
                {
                    return false;
                }

                var interfaceGenericArgs = interfaceType.GetGenericArguments();
                var typeArgs = type.GetGenericArguments();

                return interfaceGenericArgs.Length == typeArgs.Length;
            });

        public static Type FindImplementationFor<TInterface>() => FindImplementationFor(typeof(TInterface));
    }
}
