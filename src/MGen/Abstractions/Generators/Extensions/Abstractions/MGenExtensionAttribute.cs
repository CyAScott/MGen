using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

[AttributeUsage(AttributeTargets.Class), DebuggerStepThrough]
public class MGenExtensionAttribute : Attribute
{
    public static MGenExtensionAttribute Default(Type type) => new(type.Namespace == null ? type.Name : type.Namespace + "." + type.Name);

    public MGenExtensionAttribute(string id, string[]? after = null, string[]? before = null)
    {
        Id = id;

        if (after != null)
        {
            After.UnionWith(after);
        }

        if (before != null)
        {
            Before.UnionWith(before);
        }
    }

    public string Id { get; }

    public HashSet<string> After { get; } = new();

    public HashSet<string> Before { get; } = new();
}