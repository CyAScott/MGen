using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MGen.Abstractions.Builders.Members;

namespace MGen.Abstractions.Builders.Components;

public interface IHaveArgumentParameters : IHaveXmlComments
{
    ArgumentParameters ArgumentParameters { get; }

    bool ArgumentsEnabled { get; }
}

[DebuggerStepThrough]
public class ArgumentParameters : IReadOnlyDictionary<string, ArgumentParameterBuilder>, IHaveState
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _arguments.GetEnumerator();

    internal ArgumentParameters(IHaveArgumentParameters parent, char start = '(', char stop = ')', ImmutableArray<IParameterSymbol>? parameters = null)
    {
        _start = start;
        _stop = stop;
        Parent = parent;

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                if (parameter != null)
                {
                    Add(parameter);
                }
            }
        }
    }

    readonly List<KeyValuePair<string, ArgumentParameterBuilder>> _arguments = new();
    readonly char _start, _stop;

    [ExcludeFromCodeCoverage]
    public ArgumentParameterBuilder this[int index] => _arguments[index].Value;

    [ExcludeFromCodeCoverage]
    public ArgumentParameterBuilder this[string name] => _arguments[IndexOf(name)].Value;

    public ArgumentParameterBuilder Add(IParameterSymbol parameter)
    {
        var argument = new ArgumentParameterBuilder(Parent, parameter);
        if (IndexOf(parameter.Name) != -1)
        {
            throw new ArgumentException();
        }
        _arguments.Add(new KeyValuePair<string, ArgumentParameterBuilder>(argument.Name, argument));
        return argument;
    }

    public ArgumentParameterBuilder Add(Code type, string name)
    {
        if (IndexOf(name) != -1)
        {
            throw new ArgumentException();
        }
        var argument = new ArgumentParameterBuilder(Parent, type, name);
        _arguments.Add(new KeyValuePair<string, ArgumentParameterBuilder>(argument.Name, argument));
        return argument;
    }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public IHaveArgumentParameters Parent { get; }

    [ExcludeFromCodeCoverage]
    public IEnumerator<KeyValuePair<string, ArgumentParameterBuilder>> GetEnumerator() => _arguments.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public IEnumerable<ArgumentParameterBuilder> Values => _arguments.Select(it => it.Value);

    [ExcludeFromCodeCoverage]
    public IEnumerable<string> Keys => _arguments.Select(it => it.Key);

    [ExcludeFromCodeCoverage]
    public bool ContainsKey(string name) => IndexOf(name) != -1;

    [ExcludeFromCodeCoverage]
    public bool TryGetValue(string name, out ArgumentParameterBuilder value)
    {
        var index = IndexOf(name);

        if (index == -1)
        {
            value = default!;
            return false;
        }

        value = _arguments[index].Value;
        return true;
    }

    [ExcludeFromCodeCoverage]
    public int Count => _arguments.Count;

    [ExcludeFromCodeCoverage]
    public int IndexOf(string name)
    {
        for (var index = 0; index < _arguments.Count; index++)
        {
            if (_arguments[index].Key == name)
            {
                return index;
            }
        }
        return -1;
    }

    public void AppendArguments(StringBuilder stringBuilder)
    {
        if (!Parent.ArgumentsEnabled)
        {
            return;
        }

        var isFirst = true;
        stringBuilder.Append(_start);
        foreach (var argument in Values)
        {
            if (isFirst)
            {
                argument.HasThisModifier = Parent is MethodBuilder { IsExtensionMethod: true, IsExtensionMethodAllowed: true };
                isFirst = false;
            }
            else
            {
                argument.HasThisModifier = false;
                stringBuilder.Append(", ");
            }
            stringBuilder.AppendCode(argument);
        }
        stringBuilder.Append(_stop);
    }

    [ExcludeFromCodeCoverage]
    public void Remove(int index) => _arguments.RemoveAt(index);

    [ExcludeFromCodeCoverage]
    public void Remove(string name)
    {
        var index = IndexOf(name);
        if (index != -1)
        {
            _arguments.RemoveAt(index);
        }
    }
}

[DebuggerStepThrough]
public class ArgumentParameterBuilder : IAmCode,
    IHaveAName,
    IHaveAttributes,
    IHaveState
{
    internal ArgumentParameterBuilder(IHaveArgumentParameters parent, IParameterSymbol parameter)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        Attributes = new(parent, false, parameter);
        IsParams = parameter.IsParams;
        Type = new(stringBuilder => stringBuilder.AppendType(parameter.Type));
        Name = parameter.Name;
        Parent = _parent = parent;
        RefKind = parameter.RefKind;

        if (parameter.HasExplicitDefaultValue)
        {
            if (parameter.ExplicitDefaultValue == null)
            {
                DefaultValue = "null";
            }
            else if (parameter.ExplicitDefaultValue is string @string)
            {
                DefaultValue = new(stringBuilder => stringBuilder.AppendConstant(@string));
            }
            else if (parameter.ExplicitDefaultValue is bool boolean)
            {
                DefaultValue = boolean ? "true" : "false";
            }
            else
            {
                DefaultValue = new(stringBuilder => stringBuilder.Append(parameter.ExplicitDefaultValue));
            }
        }
    }

    internal ArgumentParameterBuilder(IHaveArgumentParameters parent, Code type, string name)
    {
        Attributes = new(parent, false);
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Parent = _parent = parent;
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    internal bool HasThisModifier { get; set; }

    public Attributes Attributes { get; }

    public Code? DefaultValue { get; set; }

    public Code Type { get; }

    /// <summary>
    /// The description used for XML comments.
    /// </summary>
    public List<string> Description =>
        _parent.XmlComments.Parameters.TryGetValue(Name, out var lines)
            ? lines
            : _parent.XmlComments.Parameters[Name] = new();

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode Parent { get; }

    public RefKind RefKind { get; set; } = RefKind.None;

    public bool IsParams { get; set; }

    public string Name { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        stringBuilder.AppendCode(Attributes);

        if (HasThisModifier)
        {
            stringBuilder.Append("this ");
        }
        else
        {
            switch (RefKind)
            {
                case RefKind.In:
                    stringBuilder.Append("in ");
                    break;
                case RefKind.Out:
                    stringBuilder.Append("out ");
                    break;
                case RefKind.Ref:
                    stringBuilder.Append("ref ");
                    break;
                default:
                    if (IsParams)
                    {
                        stringBuilder.Append("params ");
                    }
                    break;
            }
        }

        stringBuilder.AppendCode(Type).Append(' ').Append(Name);

        if (DefaultValue != null)
        {
            stringBuilder.Append(" = ").AppendCode(DefaultValue);
        }
    }

    readonly IHaveArgumentParameters _parent;
}
