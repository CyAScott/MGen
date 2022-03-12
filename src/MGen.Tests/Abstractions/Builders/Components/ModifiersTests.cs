using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

class ModifiersTests
{
    static IEnumerable<object[]> AddAndAllowedTestCases()
    {
        foreach (var modifier in Enum.GetValues<Modifier>())
        {
            yield return new object[]
            {
                modifier,
                false
            };
            yield return new object[]
            {
                modifier,
                true
            };
        }
    }

    [Test, TestCaseSource(nameof(AddAndAllowedTestCases))]
    public void TestAddAndAllowed(Modifier modifier, bool isAllowed)
    {
        var modifiers = isAllowed ? new Modifiers(modifier) : new Modifiers();

        Assert.AreEqual(isAllowed, modifiers.IsAllowed(modifier));

        var isAllowedProperty = typeof(Modifiers).GetProperty("Is" + modifier + "Allowed");
        Assert.IsNotNull(isAllowedProperty);
        Assert.IsTrue(isAllowedProperty.CanRead);
        Assert.IsFalse(isAllowedProperty.CanWrite);
        Assert.AreEqual(isAllowed, isAllowedProperty.GetValue(modifiers));

        Assert.IsFalse(modifiers.Contains(modifier));

        var isProperty = typeof(Modifiers).GetProperty("Is" + modifier);
        Assert.IsNotNull(isProperty);
        Assert.IsTrue(isProperty.CanRead);
        Assert.IsTrue(isProperty.CanWrite);
        Assert.IsFalse((bool)isProperty.GetValue(modifiers)!);

        if (isAllowed)
        {
            Assert.IsTrue(modifiers.Add(modifier));
            Assert.IsFalse(modifiers.Add(modifier));
            Assert.IsTrue(modifiers.Contains(modifier));
            Assert.IsTrue((bool)isProperty.GetValue(modifiers)!);
            Assert.IsTrue(modifiers.Remove(modifier));
            Assert.IsFalse(modifiers.Remove(modifier));
            Assert.IsFalse(modifiers.Contains(modifier));
            Assert.IsFalse((bool)isProperty.GetValue(modifiers)!);

            Assert.IsTrue(modifiers.Add(modifier));
            isProperty.SetValue(modifiers, true);
            Assert.IsTrue(modifiers.Contains(modifier));

            isProperty.SetValue(modifiers, false);
            Assert.IsFalse(modifiers.Contains(modifier));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => modifiers.Add(modifier));
            Assert.IsFalse(modifiers.Contains(modifier));
            Assert.IsFalse((bool)isProperty.GetValue(modifiers)!);

        }
    }
    
    [Test, TestCase(false), TestCase(true)]
    public void TestGenerate(bool appendAccessors)
    {
        var modifiers = new Modifiers(Enum.GetValues<Modifier>());

        foreach (var modifer in Enum.GetValues<Modifier>())
        {
            Assert.IsFalse(modifiers.Contains(modifer));
            Assert.IsTrue(modifiers.Add(modifer));
            Assert.IsTrue(modifiers.Contains(modifer));
        }

        var stringBuilder = new StringBuilder();

        modifiers.AppendModifiers(stringBuilder, appendAccessors);

        var code = stringBuilder.ToString();

        Assert.IsTrue(code.All(c => char.IsLetter(c) && char.IsLower(c) || c == ' '));

        var results = code
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(it => Enum.Parse<Modifier>(it, true))
            .ToList();
        
        for (var index = 1; index < results.Count; index++)
        {
            Assert.Less((int)results[index - 1], (int)results[index]);
        }

        if (appendAccessors)
        {
            Assert.That(results, Has.Member(Modifier.Public));
            Assert.That(results, Has.Member(Modifier.Private));
            Assert.That(results, Has.Member(Modifier.Protected));
            Assert.That(results, Has.Member(Modifier.Internal));
        }
        else
        {
            Assert.That(results, Has.No.Member(Modifier.Public));
            Assert.That(results, Has.No.Member(Modifier.Private));
            Assert.That(results, Has.No.Member(Modifier.Protected));
            Assert.That(results, Has.No.Member(Modifier.Internal));
        }
    }
}