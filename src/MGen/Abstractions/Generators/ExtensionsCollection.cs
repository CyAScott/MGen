using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators;

[DebuggerStepThrough]
class ExtensionsCollection<TExtension> : Dictionary<string, MGenExtensionInfo<TExtension>>
{
    public List<TExtension> ToSortedList()
    {
        var before = new HashSet<string>();
        var group = new List<MGenExtensionInfo<TExtension>>();
        var list = new List<TExtension>();

        while (Count > 0)
        {
            foreach (var extensionInfo in Values)
            {
                if (extensionInfo.Attribute.After.Count == 0)
                {
                    group.Add(extensionInfo);
                }
            }

            before.UnionWith(group.SelectMany(it => it.Attribute.Before));

            var extension = group.FirstOrDefault(it => !before.Contains(it.Attribute.Id)) ??
                            throw new InvalidProgramException("Loop detected for MGen extensions: " + string.Join(", ", group.Select(it => it.Attribute.Id).ToArray()));

            list.Add(extension.Extension);

            Remove(extension.Attribute.Id);

            foreach (var extensionInfo in Values)
            {
                extensionInfo.Attribute.After.Remove(extension.Attribute.Id);
            }

            before.Clear();
            group.Clear();
        }

        return list;
    }

    public void Add(Assembly assembly)
    {
        foreach (var extensionInfo in assembly.ExportedTypes
            .Where(it => it.IsClass && typeof(TExtension).IsAssignableFrom(it))
            .Select(it => new MGenExtensionInfo<TExtension>(
                it.GetCustomAttribute<MGenExtensionAttribute>() ?? MGenExtensionAttribute.Default(it),
                (TExtension)Activator.CreateInstance(it))))
        {
            Add(extensionInfo.Attribute.Id, extensionInfo);
        }
    }

    public void Add(TExtension extension)
    {
        var extensionType = extension!.GetType();
        var extensionInfo = new MGenExtensionInfo<TExtension>(extensionType.GetCustomAttribute<MGenExtensionAttribute>() ?? MGenExtensionAttribute.Default(extensionType), extension);
        Add(extensionInfo.Attribute.Id, extensionInfo);
    }
}

[DebuggerStepThrough]
class MGenExtensionInfo<TExtension>
{
    public MGenExtensionInfo(MGenExtensionAttribute attribute, TExtension extension)
    {
        Attribute = attribute;
        Extension = extension;
    }

    public TExtension Extension { get; }
    public MGenExtensionAttribute Attribute { get; }
}