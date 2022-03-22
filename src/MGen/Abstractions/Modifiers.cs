using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MGen.Abstractions;

public interface IHaveModifiers
{
    Modifiers Modifiers { get; }
}

public enum Modifier
{
    Public = 1,
    Private = 2,
    Protected = 3,
    Internal = 4,

    Static = 5,

    Partial = 6,
    Abstract = 7,
    Virtual = 8,
    Sealed = 9,
    Override = 10,

    Readonly = 11,
    Volatile = 12,

    New = 13,
    
    Async = 14
}

/// <summary>
/// Access modifier (i.e. public, internal, etc.) and other modifiers (i.e. static, readonly, etc.).
/// </summary>
[DebuggerStepThrough]
public partial class Modifiers : IReadOnlyCollection<Modifier>
{
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => _modifiers.GetEnumerator();

    readonly HashSet<Modifier> _allowed;
    readonly HashSet<Modifier> _modifiers = new();

    internal Modifiers(SyntaxTokenList? modifiers, params Modifier[] allowedModifiers)
        : this(allowedModifiers)
    {
        OriginalModifers = modifiers;
        if (modifiers != null)
        {
            foreach (var modifer in modifiers)
            {
                switch (modifer.ValueText)
                {
                    case "internal":
                        Add(Modifier.Internal);
                        break;
                    case "partial":
                        Add(Modifier.Partial);
                        break;
                    case "public":
                        Add(Modifier.Public);
                        break;
                    case "private":
                        Add(Modifier.Private);
                        break;
                    case "protected":
                        Add(Modifier.Protected);
                        break;
                }
            }
        }
    }
    internal Modifiers(params Modifier[] allowedModifiers) => _allowed = new(allowedModifiers);
    
    [ExcludeFromCodeCoverage]
    public IEnumerator<Modifier> GetEnumerator() => _modifiers.GetEnumerator();

    public SyntaxTokenList? OriginalModifers { get; }

    public bool Add(Modifier modifier)
    {
        if (!_allowed.Contains(modifier))
        {
            throw new ArgumentException();
        }
        
        return _modifiers.Add(modifier);
    }

    public bool IsAllowed(Modifier modifier) => _allowed.Contains(modifier);

    public bool Remove(Modifier modifier) => _modifiers.Remove(modifier);

    public bool Contains(Modifier modifier) => _modifiers.Contains(modifier);

    [ExcludeFromCodeCoverage]
    public int Count => _modifiers.Count;

    public void AppendModifiers(StringBuilder stringBuilder, bool appendAccessors = true)
    {
        foreach (var modifier in _modifiers.OrderBy(it => (int)it))
        {
            if (appendAccessors || modifier > Modifier.Internal)
            {
                stringBuilder.Append(modifier.ToString().ToLower()).Append(' ');
            }
        }
    }
}

partial class Modifiers
{
    public bool IsAbstract
    {
        get => _modifiers.Contains(Modifier.Abstract);
        set
        {
            if (value)
            {
                Add(Modifier.Abstract);
            }
            else
            {
                Remove(Modifier.Abstract);
            }
        }
    }
    public bool IsAbstractAllowed => _allowed.Contains(Modifier.Abstract);

    public bool IsAsync
    {
        get => _modifiers.Contains(Modifier.Async);
        set
        {
            if (value)
            {
                Add(Modifier.Async);
            }
            else
            {
                Remove(Modifier.Async);
            }
        }
    }
    public bool IsAsyncAllowed => _allowed.Contains(Modifier.Async);

    public bool IsInternal
    {
        get => _modifiers.Contains(Modifier.Internal);
        set
        {
            if (value)
            {
                Add(Modifier.Internal);
            }
            else
            {
                Remove(Modifier.Internal);
            }
        }
    }
    public bool IsInternalAllowed => _allowed.Contains(Modifier.Internal);

    public bool IsNew
    {
        get => _modifiers.Contains(Modifier.New);
        set
        {
            if (value)
            {
                Add(Modifier.New);
            }
            else
            {
                Remove(Modifier.New);
            }
        }
    }
    public bool IsNewAllowed => _allowed.Contains(Modifier.New);

    public bool IsOverride
    {
        get => _modifiers.Contains(Modifier.Override);
        set
        {
            if (value)
            {
                Add(Modifier.Override);
            }
            else
            {
                Remove(Modifier.Override);
            }
        }
    }
    public bool IsOverrideAllowed => _allowed.Contains(Modifier.Override);

    public bool IsPartial
    {
        get => _modifiers.Contains(Modifier.Partial);
        set
        {
            if (value)
            {
                Add(Modifier.Partial);
            }
            else
            {
                Remove(Modifier.Partial);
            }
        }
    }
    public bool IsPartialAllowed => _allowed.Contains(Modifier.Partial);

    public bool IsPrivate
    {
        get => _modifiers.Contains(Modifier.Private);
        set
        {
            if (value)
            {
                Add(Modifier.Private);
            }
            else
            {
                Remove(Modifier.Private);
            }
        }
    }
    public bool IsPrivateAllowed => _allowed.Contains(Modifier.Private);

    public bool IsProtected
    {
        get => _modifiers.Contains(Modifier.Protected);
        set
        {
            if (value)
            {
                Add(Modifier.Protected);
            }
            else
            {
                Remove(Modifier.Protected);
            }
        }
    }
    public bool IsProtectedAllowed => _allowed.Contains(Modifier.Protected);

    public bool IsPublic
    {
        get => _modifiers.Contains(Modifier.Public);
        set
        {
            if (value)
            {
                Add(Modifier.Public);
            }
            else
            {
                Remove(Modifier.Public);
            }
        }
    }
    public bool IsPublicAllowed => _allowed.Contains(Modifier.Public);

    public bool IsReadonly
    {
        get => _modifiers.Contains(Modifier.Readonly);
        set
        {
            if (value)
            {
                Add(Modifier.Readonly);
            }
            else
            {
                Remove(Modifier.Readonly);
            }
        }
    }
    public bool IsReadonlyAllowed => _allowed.Contains(Modifier.Readonly);

    public bool IsSealed
    {
        get => _modifiers.Contains(Modifier.Sealed);
        set
        {
            if (value)
            {
                Add(Modifier.Sealed);
            }
            else
            {
                Remove(Modifier.Sealed);
            }
        }
    }
    public bool IsSealedAllowed => _allowed.Contains(Modifier.Sealed);

    public bool IsStatic
    {
        get => _modifiers.Contains(Modifier.Static);
        set
        {
            if (value)
            {
                Add(Modifier.Static);
            }
            else
            {
                Remove(Modifier.Static);
            }
        }
    }
    public bool IsStaticAllowed => _allowed.Contains(Modifier.Static);

    public bool IsVirtual
    {
        get => _modifiers.Contains(Modifier.Virtual);
        set
        {
            if (value)
            {
                Add(Modifier.Virtual);
            }
            else
            {
                Remove(Modifier.Virtual);
            }
        }
    }
    public bool IsVirtualAllowed => _allowed.Contains(Modifier.Virtual);

    public bool IsVolatile
    {
        get => _modifiers.Contains(Modifier.Volatile);
        set
        {
            if (value)
            {
                Add(Modifier.Volatile);
            }
            else
            {
                Remove(Modifier.Volatile);
            }
        }
    }
    public bool IsVolatileAllowed => _allowed.Contains(Modifier.Volatile);
}