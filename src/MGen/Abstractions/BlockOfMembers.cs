using MGen.Abstractions.Builders.Components;
using MGen.Abstractions.Builders.Members;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions;

public interface IHaveMembers : IAmIndentedCode, IReadOnlyList<IAmCode>
{
    T Add<T>(T value)
        where T : IAmCode;
}

public interface IHaveMembersWithCode :
    IHaveConstructors,
    IHaveEvents,
    IHaveFields,
    IHaveMethods,
    IHaveProperties
{
}

[DebuggerStepThrough]
public abstract class BlockOfMembers : CodeCollection, IHaveMembers
{
    protected BlockOfMembers(int indentLevel)
        : base(indentLevel)
    {
    }

    protected override bool AppendBlankLinesBetweenItems => true;

    protected sealed override void AppendHeader(StringBuilder stringBuilder)
    {
        var hasArgumentParameters = this as IHaveArgumentParameters;
        var hasExplicitDeclaration = this as ICanHaveAnExplicitDeclaration;
        var hasGenericParameters = this as IHaveGenericParameters;

        if (this is IHaveUsings hasUsings)
        {
            stringBuilder.AppendCode(hasUsings.Usings);
        }

        if (this is IHaveXmlComments hasXmlSummary)
        {
            stringBuilder.AppendCode(hasXmlSummary.XmlComments);
        }

        if (this is IHaveAttributes hasAttributes)
        {
            stringBuilder.AppendCode(hasAttributes.Attributes);
        }

        stringBuilder.AppendIndent(IndentLevel);

        if (this is IHaveModifiers hasModifiers)
        {
            hasModifiers.Modifiers.AppendModifiers(stringBuilder, hasExplicitDeclaration is not
            {
                ExplicitDeclaration.IsExplicitDeclarationEnabled: true
            });
        }

        if (this is IHaveADeclarationKeyword hasAKeyword)
        {
            stringBuilder.Append(hasAKeyword.Keyword).Append(' ');
        }

        if (this is IHaveAReturnType hasReturnType)
        {
            stringBuilder.AppendCode(hasReturnType.ReturnType).Append(' ');
        }

        stringBuilder.AppendCode(hasExplicitDeclaration?.ExplicitDeclaration);

        if (this is IHaveAName hasName)
        {
            stringBuilder.Append(hasName.Name);
        }

        hasGenericParameters?.GenericParameters.AppendParameters(stringBuilder);

        if (this is IHaveInheritance hasInheritance)
        {
            stringBuilder.AppendCode(hasInheritance.Inheritance);
        }

        hasArgumentParameters?.ArgumentParameters.AppendArguments(stringBuilder);

        if (hasGenericParameters != null && hasGenericParameters.GenericParameters.HasConstraints)
        {
            stringBuilder.AppendLine();
            hasGenericParameters.GenericParameters.AppendConstraints(stringBuilder);
        }

        if (AppendLineAtEol)
        {
            stringBuilder.AppendLine();
        }
    }

    protected virtual bool AppendLineAtEol => true;
}