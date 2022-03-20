using MGen.Abstractions.Builders.Components;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveProperties : IHaveModifiers, IHaveTypes
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static PropertyBuilder AddProperty(this IHaveProperties members, IPropertySymbol first, IPropertySymbol? second = null)
    {
        var property = second == null ? members.Add(new PropertyBuilder(members, first)) : members.Add(new PropertyBuilder(members, first, second));

        if (property.Field != null)
        {
            members.Add(property.Field);
        }

        return property;
    }

    [DebuggerStepThrough]
    public static PropertyBuilder AddIndexProperty(this IHaveProperties members, string propertyType, string indexType, string indexName)
    {
        var property = members.Add(new PropertyBuilder(members, propertyType, "this", false));

        property.ArgumentParameters.Add(indexType, indexName);

        if (property.Field != null)
        {
            members.Add(property.Field);
        }

        return property;
    }

    [DebuggerStepThrough]
    public static PropertyBuilder AddProperty(this IHaveProperties members, string type, string name, bool generateField = true)
    {
        var property = members.Add(new PropertyBuilder(members, type, name, generateField));

        if (property.Field != null)
        {
            members.Add(property.Field);
        }

        return property;
    }
}

[DebuggerStepThrough]
public sealed class PropertyBuilder : BlockOfMembers,
    ICanHaveAnExplicitDeclaration,
    IHaveAReturnType,
    IHaveArgumentParameters,
    IHaveAttributes,
    IHaveAName,
    IHaveCodeGenerators,
    IHaveModifiers,
    IHaveState
{
    internal PropertyBuilder(IHaveProperties parent, params IPropertySymbol[] properties)
        : base(parent.IndentLevel + 1)
    {
        var property = properties[0];

        _parent = parent;
        ArgumentParameters = new(this, '[', ']', property.IsIndexer ? property.Parameters : null);
        Attributes = new(this, true, property);
        ExplicitDeclaration = new(property);
        Modifiers = new(Modifier.Public)
        {
            IsPublic = true
        };
        Name = property.Name;
        PropertySymbols = properties;
        ReturnType = new CodeType(property.Type);
        XmlComments = new(this, property);

        var getMethod = properties.Select(it => it.GetMethod).FirstOrDefault(it => it != null);
        Add(Get = new(this, true, getMethod)
        {
            Enabled = getMethod != null
        });

        var setMethod = properties.Select(it => it.SetMethod).FirstOrDefault(it => it != null);
        Add(Set = new(this, false, setMethod)
        {
            Enabled = setMethod != null
        });

        if (!property.IsIndexer && parent is IHaveFields item)
        {
            var fields = item.OfType<IHaveAName>().Select(it => it.Name).ToList();

            var fieldName = CreateFieldName(fields, Name);

            Field = new FieldBuilder(item, ReturnType, fieldName)
            {
                Modifiers =
                {
                    IsReadonly = !Set.Enabled
                }
            };
        }
    }

    internal PropertyBuilder(IHaveProperties parent, Code type, string name, bool generateField = true)
        : base(parent.IndentLevel + 1)
    {
        _parent = parent;
        ArgumentParameters = new(this, '[', ']');
        Attributes = new(this, true);
        ExplicitDeclaration = new();
        XmlComments = new(this);
        if (parent.Modifiers.IsAbstractAllowed)
        {
            Modifiers = new(Modifier.Abstract, Modifier.Internal, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Sealed, Modifier.Static);
        }
        else
        {
            Modifiers = new(Modifier.Internal, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Sealed, Modifier.Static);
        }
        Name = name;
        PropertySymbols = Array.Empty<IPropertySymbol>();
        ReturnType = type;

        Add(Get = new(this, true));
        Add(Set = new(this, false));

        if (generateField && parent is IHaveFields item)
        {
            var fields = item.OfType<IHaveAName>().Select(it => it.Name).ToList();

            var fieldName = CreateFieldName(fields, name);

            Field = new FieldBuilder(item, type, fieldName);
        }
    }

    internal static string CreateFieldName(List<string> fields, string propertyName)
    {
        var name = "_" + Regex.Replace(propertyName, "^[A-Z]+", match =>
            match.Length == 1 || match.Length == propertyName.Length ? match.Value.ToLower() :
                match.Value.Substring(0, match.Length - 1).ToLower() + match.Value[match.Length - 1], RegexOptions.Compiled);

        while (fields.Contains(name))
        {
            name = "_" + name;
        }

        return name;
    }

    protected override bool AppendLineAtEol => Get.IsBodyEnabled || Set.IsBodyEnabled;

    protected override void AppendBody(StringBuilder stringBuilder)
    {
        if (Get.IsBodyEnabled || Set.IsBodyEnabled)
        {
            stringBuilder.AppendIndent(IndentLevel).AppendLine("{");
            stringBuilder.AppendCode(Get);
            stringBuilder.AppendCode(Set);
            stringBuilder.AppendIndent(IndentLevel).Append('}');
        }
        else
        {
            stringBuilder.Append(" {");

            if (Get.Enabled)
            {
                stringBuilder.Append(' ').AppendCode(Get);
            }

            if (Set.Enabled)
            {
                stringBuilder.Append(' ').AppendCode(Set);
            }

            stringBuilder.Append(" }");
        }

        if (_initializer == null)
        {
            stringBuilder.AppendLine();
        }
        else
        {
            stringBuilder.Append(" = ").AppendCode(_initializer).AppendLine(";");
        }

        if (Field != null && !Set.Enabled)
        {
            Field.Modifiers.IsReadonly = true;
        }
    }

    public ArgumentParameters ArgumentParameters { get; }

    public Components.Attributes Attributes { get; }

    public Code? Initializer
    {
        get => Field?.Initializer ?? _initializer;
        set
        {
            if (Field == null)
            {
                _initializer = value;
            }
            else
            {
                Field.Initializer = value;
            }
        }
    }
    Code? _initializer;

    public Code ReturnType { get; set; }

    public CodeGenerators CodeGenerators => _parent.CodeGenerators;

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public ExplicitDeclaration ExplicitDeclaration { get; }

    public FieldBuilder? Field { get; }

    public IAmIndentedCode Parent => _parent;
    readonly IHaveProperties _parent;

    public IPropertySymbol[] PropertySymbols { get; }

    public PropertyGetterSetter Get { get; }

    public PropertyGetterSetter Set { get; }

    public Modifiers Modifiers { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public bool ArgumentsEnabled => ArgumentParameters.Count > 0;

    public override bool Enabled
    {
        get => Field?.Enabled ?? _enabled;
        set
        {
            if (Field == null)
            {
                _enabled = value;
            }
            else
            {
                Field.Enabled = value;
            }
        }
    }
    bool _enabled = true;

    public string Name { get; }
    string IHaveAName.Name => ArgumentParameters.Count == 0 ? Name : "this";

    public void GenerateCode() => _parent.CodeGenerators.GenerateCode(this);
}

[DebuggerStepThrough]
public class PropertyGetterSetter : BlockOfCode<PropertyBuilder>, IHaveAttributes, IHaveState
{
    internal PropertyGetterSetter(PropertyBuilder parent, bool isGetter, IMethodSymbol? method = null)
        : base(parent, parent.IndentLevel + 1)
    {
        Attributes = new(this, true, method);
        IsGetter = isGetter;
        Modifiers = method == null ? new(Modifier.Internal, Modifier.Private, Modifier.Protected) : new();
    }

    protected internal override bool IsBodyEnabled => Enabled && Count > 0 && !Parent.Modifiers.IsAbstract && (Parent.Parent is not InterfaceBuilder || Parent.Modifiers.Count > 0);

    protected override void AppendHeader(StringBuilder stringBuilder)
    {
        var isBodyEnabled = IsBodyEnabled;

        Attributes.AppendNewLineBetweenEachAttribute = isBodyEnabled;

        stringBuilder.AppendCode(Attributes);

        if (isBodyEnabled)
        {
            stringBuilder.AppendIndent(IndentLevel);
        }
        else if (Attributes.Count > 0)
        {
            stringBuilder.Append(' ');
        }

        if (Modifiers.IsPrivate)
        {
            stringBuilder.Append("private ");
        }

        if (Modifiers.IsProtected)
        {
            stringBuilder.Append("protected ");
        }

        if (Modifiers.IsInternal)
        {
            stringBuilder.Append("internal ");
        }

        stringBuilder.Append(IsGetter ? "get" : "set");

        if (isBodyEnabled)
        {
            stringBuilder.AppendLine();
            return;
        }

        stringBuilder.Append(";");

        if (IsGetter)
        {
            if (Parent.Set.IsBodyEnabled)
            {
                stringBuilder.AppendLine();
            }
        }
        else if (Parent.Get.IsBodyEnabled)
        {
            stringBuilder.AppendLine();
        }
    }

    public Components.Attributes Attributes { get; }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public Modifiers Modifiers { get; }

    public bool IsGetter { get; }

    public bool IsSetter => !IsGetter;

    public override void Generate(StringBuilder stringBuilder)
    {
        if (Enabled)
        {
            base.Generate(stringBuilder);
        }
    }
}