using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Components;
using Microsoft.CodeAnalysis;
using System.Text;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveEvents : IHaveAName, IHaveMembers
{
}

public static partial class MembersExtensions
{
    [DebuggerStepThrough]
    public static EventBuilder AddEvent(this IHaveEvents members, IEventSymbol @event) =>
        members.Add(new EventBuilder(members, @event));

    [DebuggerStepThrough]
    public static EventBuilder AddEvent(this IHaveEvents members, string type, string name) => members
        .Add(new EventBuilder(members, type, name));
}

[DebuggerStepThrough]
public class EventBuilder :
    ICanHaveAnExplicitDeclaration,
    IHaveADeclarationKeyword,
    IHaveAName,
    IHaveAReturnType,
    IHaveAttributes,
    IHaveEnabled,
    IHaveModifiers,
    IHaveState,
    IHaveXmlComments
{
    internal EventBuilder(IHaveEvents parent, IEventSymbol @event)
    {
        Attributes = new(this, true, @event);
        ExplicitDeclaration = new(@event);
        EventSymbol = @event;
        XmlComments = new(this, @event);
        IndentLevel = parent.IndentLevel + 1;
        Modifiers = new(Modifier.Public)
        {
            IsPublic = true
        };
        Name = @event.Name;
        Parent = parent;
        ReturnType = new CodeType(@event.Type);
    }

    internal EventBuilder(IHaveEvents parent, string type, string name)
    {
        Attributes = new(this, true);
        ExplicitDeclaration = new();
        XmlComments = new(this);
        IndentLevel = parent.IndentLevel + 1;
        Modifiers = new(Modifier.Internal, Modifier.Private, Modifier.Protected, Modifier.Public, Modifier.Static);
        Name = name;
        Parent = parent;
        ReturnType = type;
    }

    public Components.Attributes Attributes { get; }

    public Code ReturnType { get; set; }

    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public ExplicitDeclaration ExplicitDeclaration { get; }

    [ExcludeFromCodeCoverage]
    public IAmIndentedCode Parent { get; }

    public IEventSymbol? EventSymbol { get; }

    public Modifiers Modifiers { get; }

    public XmlCommentsBuilder XmlComments { get; }

    public bool Enabled { get; set; } = true;

    public int IndentLevel { get; }

    public string Keyword => "event";

    public string Name { get; }

    public void Generate(StringBuilder stringBuilder)
    {
        if (!Enabled)
        {
            return;
        }

        stringBuilder.AppendCode(XmlComments);

        stringBuilder.AppendCode(Attributes);

        stringBuilder.AppendIndent(IndentLevel);

        Modifiers.AppendModifiers(stringBuilder, !ExplicitDeclaration.IsExplicitDeclarationEnabled);

        stringBuilder.Append(Keyword).Append(' ').AppendCode(ReturnType).Append(' ');

        stringBuilder.AppendCode(ExplicitDeclaration);

        stringBuilder.Append(Name);

        if (!ExplicitDeclaration.IsExplicitDeclarationEnabled)
        {
            stringBuilder.AppendLine(";");
            return;
        }

        stringBuilder.AppendLine()
            .AppendIndent(IndentLevel).AppendLine("{")
            .AppendIndent(IndentLevel + 1).AppendLine("add => throw new System.NotImplementedException();")
            .AppendIndent(IndentLevel + 1).AppendLine("remove => throw new System.NotImplementedException();")
            .AppendIndent(IndentLevel).AppendLine("}");

    }
}