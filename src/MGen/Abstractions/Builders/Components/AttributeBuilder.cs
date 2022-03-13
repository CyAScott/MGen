using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Components;

public interface IHaveAttributes
{
    Attributes Attributes { get; }
}

[DebuggerStepThrough]
public class Attributes : IAmCode, IHaveState, IReadOnlyCollection<AttributeBuilder>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _attributes.GetEnumerator();

    internal Attributes(IAmIndentedCode parent, bool appendNewLineBetweenEachAttribute, ISymbol? symbol = null)
    {
        AppendNewLineBetweenEachAttribute = appendNewLineBetweenEachAttribute;
        Parent = parent;

        if (symbol != null)
        {
            foreach (var attributeData in symbol.GetAttributes())
            {
                if (attributeData != null)
                {
                    Add(attributeData);
                }
            }
        }
    }

    protected internal bool AppendNewLineBetweenEachAttribute { get; set; }

    [ExcludeFromCodeCoverage]
    public AttributeBuilder this[int index] => _attributes[index];

    public AttributeBuilder Add(AttributeData attribute)
    {
        var item = new AttributeBuilder(attribute);
        _attributes.Add(item);
        return item;
    }

    public AttributeBuilder Add(string type)
    {
        var item = new AttributeBuilder(type);
        _attributes.Add(item);
        return item;
    }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public IAmIndentedCode Parent { get; }

    [ExcludeFromCodeCoverage]
    public IEnumerator<AttributeBuilder> GetEnumerator() => _attributes.GetEnumerator();

    [ExcludeFromCodeCoverage]
    public int Count => _attributes.Count;

    public void Generate(StringBuilder stringBuilder)
    {
        foreach (var attribute in _attributes)
        {
            if (attribute.Enabled)
            {
                if (AppendNewLineBetweenEachAttribute)
                {
                    stringBuilder.AppendIndent(Parent.IndentLevel);
                }
                stringBuilder.AppendCode(attribute);
                if (AppendNewLineBetweenEachAttribute)
                {
                    stringBuilder.AppendLine();
                }
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public void Remove(AttributeBuilder attribute) => _attributes.Remove(attribute);

    readonly List<AttributeBuilder> _attributes = new();
}

[DebuggerStepThrough]
public class AttributeBuilder : IAmCode, IHaveEnabled, IHaveState
{
    internal AttributeBuilder(AttributeData attribute)
    {
        Type = new(attribute.AttributeClass);

        Add(attribute.ConstructorArguments);

        Add(attribute.NamedArguments);
    }

    internal AttributeBuilder(string type) => Type = type ?? throw new ArgumentNullException(nameof(type));

    public Code Type { get; }

    public Dictionary<string, Code> NamedParameters { get; } = new();
    
    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public List<Code> Arguments { get; } = new();

    public bool Enabled { get; set; } = true;

    public void Generate(StringBuilder stringBuilder)
    {
        if (!Enabled)
        {
            return;
        }

        stringBuilder.Append('[').AppendCode(Type);

        if (Arguments.Count > 0 || NamedParameters.Count > 0)
        {
            stringBuilder.Append('(');

            AppendArguments(stringBuilder);

            AppendNamedParameters(stringBuilder);

            stringBuilder.Append(')');
        }

        stringBuilder.Append(']');
    }

    void Add(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
    {
        foreach (var argument in namedArguments)
        {
            NamedParameters[argument.Key] = argument.Value;
        }
    }

    void Add(ImmutableArray<TypedConstant> constructorArguments)
    {
        foreach (var constant in constructorArguments)
        {
            Arguments.Add(constant);
        }
    }

    void AppendArguments(StringBuilder stringBuilder)
    {
        var isFirst = true;

        foreach (var argument in Arguments)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.AppendCode(argument);
        }
    }

    void AppendNamedParameters(StringBuilder stringBuilder)
    {
        var isFirst = Arguments.Count == 0;

        foreach (var parameter in NamedParameters)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(parameter.Key).Append(" = ").AppendCode(parameter.Value);
        }
    }
}