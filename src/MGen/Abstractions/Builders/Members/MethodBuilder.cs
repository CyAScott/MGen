using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Components;
using Microsoft.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveMethods : IHaveModifiers, IHaveTypes
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static MethodBuilder AddMethod(this IHaveMethods members, IMethodSymbol method) =>
        members.Add(new MethodBuilder(members, method));

    [DebuggerStepThrough]
    public static MethodBuilder AddMethod(this IHaveMethods members, string type, string name) => members
        .Add(new MethodBuilder(members, type, name));
}

[DebuggerStepThrough]
public class MethodBuilder : BlockOfCode<IHaveMethods>,
    ICanHaveAnExplicitDeclaration,
    IHaveAReturnType,
    IHaveArgumentParameters,
    IHaveAttributes,
    IHaveAName,
    IHaveGenericParameters,
    IHaveModifiers,
    IHaveState
{
    internal MethodBuilder(IHaveMethods parent, IMethodSymbol method)
        : base(parent, parent.IndentLevel + 1)
    {
        _parent = parent;
        ArgumentParameters = new(this, parameters: method.Parameters);
        Attributes = new(this, true, method);
        ExplicitDeclaration = new(method);
        GenericParameters = new(this, method.TypeArguments);
        MethodSymbol = method;
        Modifiers = parent is IHaveModifiers item && item.Modifiers.IsPartial ?
            new(Modifier.Async, Modifier.Partial, Modifier.Public)
            {
                IsPartial = true,
                IsPublic = true
            } :
            new(Modifier.Async, Modifier.Public)
            {
                IsPublic = true
            };
        Name = method.Name;
        ReturnType = new CodeType(method.ReturnType);
        XmlComments = new(this, method);
    }

    internal MethodBuilder(IHaveMethods parent, Code returnType, string name)
        : base(parent, parent.IndentLevel + 1)
    {
        _parent = parent;
        ArgumentParameters = new(this);
        Attributes = new(this, true);
        ExplicitDeclaration = new();
        GenericParameters = new(this);
        if (parent.Modifiers.IsAbstractAllowed)
        {
            Modifiers = new(Modifier.Abstract, Modifier.Async, Modifier.Internal, Modifier.Partial, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Sealed, Modifier.Static);
        }
        else
        {
            Modifiers = new(Modifier.Async, Modifier.Internal, Modifier.Partial, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Sealed, Modifier.Static);
        }
        Name = name;
        ReturnType = returnType;
        XmlComments = new(this);
    }

    protected internal override bool IsBodyEnabled => !Modifiers.IsAbstract && !Modifiers.IsPartial && (Parent is not InterfaceBuilder || Modifiers.Count > 0);
    
    protected override void AppendHeader(StringBuilder stringBuilder)
    {
        stringBuilder.AppendCode(XmlComments);

        stringBuilder.AppendCode(Attributes);

        stringBuilder.AppendIndent(IndentLevel);

        Modifiers.AppendModifiers(stringBuilder, !ExplicitDeclaration.IsExplicitDeclarationEnabled);

        stringBuilder.AppendCode(ReturnType).Append(' ');

        stringBuilder.AppendCode(ExplicitDeclaration);

        stringBuilder.Append(Name);

        GenericParameters.AppendParameters(stringBuilder);

        ArgumentParameters.AppendArguments(stringBuilder);

        if (GenericParameters.HasConstraints)
        {
            stringBuilder.AppendLine();
            GenericParameters.AppendConstraints(stringBuilder);
        }

        if (IsBodyEnabled)
        {
            stringBuilder.AppendLine();
        }
        else
        {
            stringBuilder.AppendLine(";");
        }
    }

    public ArgumentParameters ArgumentParameters { get; }

    public Components.Attributes Attributes { get; }

    public Code ReturnType { get; set; }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public ExplicitDeclaration ExplicitDeclaration { get; }

    public GenericParameters GenericParameters { get; }

    public IMethodSymbol? MethodSymbol { get; }

    public Modifiers Modifiers { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public bool IsExtensionMethod { get; set; }

    public bool IsExtensionMethodAllowed => Modifiers.IsStatic && Parent is ClassBuilder @class && @class.Modifiers.IsStatic;

    public bool ArgumentsEnabled => true;

    public string Name { get; }

    public void GenerateCode() => _parent.CodeGenerators.GenerateCode(this);

    readonly IHaveMethods _parent;
}