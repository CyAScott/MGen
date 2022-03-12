using System;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions;

public interface ICanHaveAnExplicitDeclaration
{
    ExplicitDeclaration ExplicitDeclaration { get; }
}

[DebuggerStepThrough]
public class ExplicitDeclaration : IAmCode
{
    internal ExplicitDeclaration(ISymbol? containingSymbol = null) => ContainingSymbol = containingSymbol;

    public ISymbol? ContainingSymbol { get; }

    public bool IsExplicitDeclarationEnabled { get; set; }

    public void SetExplicitDeclaration(string type)
    {
        IsExplicitDeclarationEnabled = true;
        _explicitDeclarationType = type ?? throw new ArgumentNullException(nameof(type));
    }

    public void Generate(StringBuilder stringBuilder)
    {
        if (!IsExplicitDeclarationEnabled)
        {
            return;
        }

        if (_explicitDeclarationType != null)
        {
            stringBuilder.Append(_explicitDeclarationType).Append('.');
        }
        else if (ContainingSymbol != null)
        {
            foreach (var part in ContainingSymbol.ToDisplayParts())
            {
                if (part.Kind is SymbolDisplayPartKind.EventName or
                    SymbolDisplayPartKind.MethodName or
                    SymbolDisplayPartKind.PropertyName)
                {
                    break;
                }

                if (part.Kind == SymbolDisplayPartKind.Punctuation)
                {
                    stringBuilder.Append('.');
                }
                else
                {
                    stringBuilder.Append(part.Symbol?.Name);
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Unable to explicitly declare member.");
        }
    }

    string? _explicitDeclarationType;
}