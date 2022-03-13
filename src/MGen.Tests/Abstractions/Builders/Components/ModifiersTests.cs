using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

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

        isAllowed.ShouldBe(modifiers.IsAllowed(modifier));

        var isAllowedProperty = typeof(Modifiers).GetProperty("Is" + modifier + "Allowed");
        isAllowedProperty.ShouldNotBeNull();
        isAllowedProperty.CanRead.ShouldBeTrue();
        isAllowedProperty.CanWrite.ShouldBeFalse();
        isAllowed.ShouldBe(isAllowedProperty.GetValue(modifiers));

        modifiers.ShouldNotContain(modifier);

        var isProperty = typeof(Modifiers).GetProperty("Is" + modifier);
        isProperty.ShouldNotBeNull();
        isProperty.CanRead.ShouldBeTrue();
        isProperty.CanWrite.ShouldBeTrue();
        isProperty.GetValue(modifiers).ShouldBe(false);

        if (isAllowed)
        {
            modifiers.Add(modifier).ShouldBeTrue();
            modifiers.Add(modifier).ShouldBeFalse();
            modifiers.ShouldContain(modifier);
            isProperty.GetValue(modifiers).ShouldBe(true);
            modifiers.Remove(modifier).ShouldBeTrue();
            modifiers.Remove(modifier).ShouldBeFalse();
            modifiers.ShouldNotContain(modifier);
            isProperty.GetValue(modifiers).ShouldBe(false);

            modifiers.Add(modifier).ShouldBeTrue();
            isProperty.SetValue(modifiers, true);
            modifiers.ShouldContain(modifier);

            isProperty.SetValue(modifiers, false);
            modifiers.ShouldNotContain(modifier);
        }
        else
        {
            Should.Throw<ArgumentException>(() => modifiers.Add(modifier));
            modifiers.ShouldNotContain(modifier);
            isProperty.GetValue(modifiers).ShouldBe(false);

        }
    }
    
    [Test, TestCase(false), TestCase(true)]
    public void TestGenerate(bool appendAccessors)
    {
        var modifiers = new Modifiers(Enum.GetValues<Modifier>());

        foreach (var modifer in Enum.GetValues<Modifier>())
        {
            modifiers.ShouldNotContain(modifer);
            modifiers.Add(modifer).ShouldBeTrue();
            modifiers.ShouldContain(modifer);
        }

        var stringBuilder = new StringBuilder();

        modifiers.AppendModifiers(stringBuilder, appendAccessors);

        var code = stringBuilder.ToString();

        code.All(c => char.IsLetter(c) && char.IsLower(c) || c == ' ').ShouldBeTrue();

        var results = code
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(it => Enum.Parse<Modifier>(it, true))
            .ToList();
        
        for (var index = 1; index < results.Count; index++)
        {
            var a = (int)results[index - 1];
            a.ShouldBeLessThan((int)results[index]);
        }

        if (appendAccessors)
        {
            results.ShouldContain(Modifier.Public);
            results.ShouldContain(Modifier.Public);
            results.ShouldContain(Modifier.Private);
            results.ShouldContain(Modifier.Protected);
            results.ShouldContain(Modifier.Internal);
        }
        else
        {
            results.ShouldNotContain(Modifier.Public);
            results.ShouldNotContain(Modifier.Private);
            results.ShouldNotContain(Modifier.Protected);
            results.ShouldNotContain(Modifier.Internal);
        }
    }
}