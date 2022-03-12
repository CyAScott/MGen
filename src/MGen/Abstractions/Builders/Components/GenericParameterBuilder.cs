using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MGen.Abstractions.Builders.Components;

public interface IHaveGenericParameters : IHaveXmlComments
{
    GenericParameters GenericParameters { get; }
}

[DebuggerStepThrough]
public class GenericParameters : IHaveState, IReadOnlyDictionary<string, GenericParameterBuilder>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();

    internal GenericParameters(IHaveGenericParameters parent, ImmutableArray<ITypeSymbol>? arguments = null)
    {
        Parent = parent;

        if (arguments != null)
        {
            foreach (var argument in arguments)
            {
                if (argument != null)
                {
                    Add(argument);
                }
            }
        }
    }

    readonly List<KeyValuePair<string, GenericParameterBuilder>> _parameters = new();
    
    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    [ExcludeFromCodeCoverage]
    public GenericParameterBuilder this[int index] => _parameters[index].Value;

    [ExcludeFromCodeCoverage]
    public GenericParameterBuilder this[string name] => _parameters[IndexOf(name)].Value;

    public GenericParameterBuilder Add(ITypeSymbol type)
    {
        var parameter = new GenericParameterBuilder(Parent, type);
        if (IndexOf(parameter.Name) != -1)
        {
            throw new ArgumentException();
        }
        _parameters.Add(new KeyValuePair<string, GenericParameterBuilder>(parameter.Name, parameter));
        return parameter;
    }

    public GenericParameterBuilder Add(string name, string? constraint = null)
    {
        if (IndexOf(name) != -1)
        {
            throw new ArgumentException();
        }
        var parameter = new GenericParameterBuilder(Parent, name, constraint);
        _parameters.Add(new KeyValuePair<string, GenericParameterBuilder>(parameter.Name, parameter));
        return parameter;
    }

    public IHaveGenericParameters Parent { get; }

    [ExcludeFromCodeCoverage]
    public IEnumerable<GenericParameterBuilder> Values => _parameters.Select(it => it.Value);

    [ExcludeFromCodeCoverage]
    public IEnumerator<KeyValuePair<string, GenericParameterBuilder>> GetEnumerator() => _parameters.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public IEnumerable<string> Keys => _parameters.Select(it => it.Key);

    [ExcludeFromCodeCoverage]
    public bool ContainsKey(string name) => IndexOf(name) != -1;

    public bool HasConstraints => _parameters.Any(it => it.Value.Constraint != null);

    [ExcludeFromCodeCoverage]
    public bool TryGetValue(string name, out GenericParameterBuilder value)
    {
        var index = IndexOf(name);

        if (index == -1)
        {
            value = default!;
            return false;
        }

        value = _parameters[index].Value;
        return true;
    }

    [ExcludeFromCodeCoverage]
    public int Count => _parameters.Count;

    [ExcludeFromCodeCoverage]
    public int IndexOf(string name)
    {
        for (var index = 0; index < _parameters.Count; index++)
        {
            if (_parameters[index].Key == name)
            {
                return index;
            }
        }
        return -1;
    }

    public void AppendConstraints(StringBuilder stringBuilder)
    {
        var constraintCount = _parameters.Count(it => it.Value.Constraint != null);

        foreach (var parameter in _parameters)
        {
            if (parameter.Value.Constraint == null)
            {
                continue;
            }

            var indentLevel = Parent.IndentLevel + 1;

            stringBuilder
                .AppendIndent(indentLevel)
                .Append("where ")
                .Append(parameter.Value.Name)
                .Append(" : ")
                .AppendCode(parameter.Value.Constraint!);

            constraintCount--;

            if (constraintCount > 0)
            {
                stringBuilder.AppendLine();
            }
        }
    }

    public void AppendParameters(StringBuilder stringBuilder)
    {
        if (_parameters.Count == 0)
        {
            return;
        }

        var isFirst = true;
        stringBuilder.Append('<');
        foreach (var parameter in Values)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.AppendCode(parameter.Attributes).Append(parameter.Name);
        }
        stringBuilder.Append('>');
    }

    [ExcludeFromCodeCoverage]
    public void Remove(int index) => _parameters.RemoveAt(index);

    [ExcludeFromCodeCoverage]
    public void Remove(string name)
    {
        var index = IndexOf(name);
        if (index != -1)
        {
            _parameters.RemoveAt(index);
        }
    }
}

[DebuggerStepThrough]
public class GenericParameterBuilder :
    IHaveAName,
    IHaveAttributes,
    IHaveState
{
    bool TryToAppendPointerConstraint(StringBuilder stringBuilder, ITypeParameterSymbol parameter)
    {
        if (parameter.HasReferenceTypeConstraint)
        {
            stringBuilder.Append("class");

            if (parameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated)
            {
                stringBuilder.Append('?');
            }

            return true;
        }

        if (parameter.HasValueTypeConstraint)
        {
            stringBuilder.Append("struct");
            return true;
        }

        if (parameter.HasNotNullConstraint)
        {
            stringBuilder.Append("notnull");
            return true;
        }

        return false;
    }

    void AppendGenericConstraint(StringBuilder stringBuilder, ITypeParameterSymbol parameter)
    {
        var hasParts = TryToAppendPointerConstraint(stringBuilder, parameter);

        if (parameter.ConstraintTypes.Length > 0)
        {
            if (hasParts)
            {
                stringBuilder.Append(", ");
            }

            AppendTypeConstraints(stringBuilder, parameter);

            hasParts = true;
        }

        if (parameter.HasConstructorConstraint)
        {
            if (hasParts)
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append("new()");
        }
    }

    void AppendTypeConstraints(StringBuilder stringBuilder, ITypeParameterSymbol parameter)
    {
        for (var index = 0; index < parameter.ConstraintTypes.Length; index++)
        {
            if (index > 0)
            {
                stringBuilder.Append(", ");
            }
            stringBuilder.AppendType(parameter.ConstraintTypes[index]);
        }
    }

    internal GenericParameterBuilder(IHaveGenericParameters parent, ITypeSymbol type)
    {
        if (type is ITypeParameterSymbol parameter &&
            (
                parameter.ConstraintNullableAnnotations.Length > 0 ||parameter.ConstraintTypes.Length > 0 ||
                parameter.HasConstructorConstraint || parameter.HasNotNullConstraint ||
                parameter.HasReferenceTypeConstraint || parameter.HasValueTypeConstraint ||
                parameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated
            ))
        {
            Constraint = new(stringBuilder => AppendGenericConstraint(stringBuilder, parameter));
        }

        Attributes = new(parent, false, type);
        Name = type.Name;
        _parent = parent;
    }

    internal GenericParameterBuilder(IHaveGenericParameters parent, string name, string? constraint = null)
    {
        Attributes = new(_parent = parent, false);
        Name = name;
        Constraint = constraint == null ? null : (Code)constraint;
    }

    public Attributes Attributes { get; }

    public Code? Constraint { get; }

    /// <summary>
    /// The description used for XML comments.
    /// </summary>
    public List<string> Description =>
        _parent.XmlComments.TypeParameters.TryGetValue(Name, out var lines)
            ? lines
            : _parent.XmlComments.TypeParameters[Name] = new();

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public string Name { get; }

    readonly IHaveGenericParameters _parent;
}