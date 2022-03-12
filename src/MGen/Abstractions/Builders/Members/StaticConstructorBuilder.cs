using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MGen.Abstractions.Builders.Components;
using System.Text;

namespace MGen.Abstractions.Builders.Members;

public interface IHaveAStaticConstructor : IHaveAName, IHaveMembers
{
    StaticConstructorBuilder StaticConstructor { get; }
}

public sealed class StaticConstructorBuilder : BlockOfCode<IHaveAStaticConstructor>, IHaveADeclarationKeyword, IHaveAName, IHaveAttributes, IHaveState
{
    internal StaticConstructorBuilder(IHaveAStaticConstructor parent)
        : base(parent, parent.IndentLevel + 1)
    {
        _parent = parent;
        Attributes = new(this, true);
        Enabled = false;
    }

    protected override void AppendHeader(StringBuilder stringBuilder)
    {
        stringBuilder.AppendCode(Attributes);

        stringBuilder.AppendIndent(IndentLevel).Append("static ").Append(Name).AppendLine("()");
    }

    public Components.Attributes Attributes { get; }
    
    [ExcludeFromCodeCoverage]
    public Dictionary<string, object> State { get; } = new();

    public string Name => _parent.Name;

    public string Keyword => "static";

    readonly IHaveAStaticConstructor _parent;
}
