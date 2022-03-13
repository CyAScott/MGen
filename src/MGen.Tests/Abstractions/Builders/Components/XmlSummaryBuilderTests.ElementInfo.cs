using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using Shouldly;
using ElementClosure = MGen.Abstractions.Builders.Components.XmlCommentsBuilder.ElementClosure;
using ElementInfo = MGen.Abstractions.Builders.Components.XmlCommentsBuilder.ElementInfo;

namespace MGen.Abstractions.Builders.Components;

partial class XmlSummaryBuilderTests
{
    ElementInfo Create(string element, string? after = null) => new(
        ReadOnlySpan<char>.Empty,
        element.AsSpan(),
        after == null ? ReadOnlySpan<char>.Empty : after.AsSpan());

    [Test,
     TestCase("<summary>Line <param>1</param>\r\nLine 2</summary>", "Line <param>1</param>", "Line 2")]
    public void TestCopyContentsTo(string @case, params string[] expectedLines)
    {
        ElementInfo.Next(@case.AsSpan(), out var element);

        var actualLines = new List<string>();

        element.CopyContentsTo(actualLines, out _);

        expectedLines.Length.ShouldBe(actualLines.Count);

        for (var index = 0; index < actualLines.Count; index++)
        {
            expectedLines[index].ShouldBe(actualLines[index]);
        }
    }

    [Test]
    public void TestElementWithNoTermination()
    {
        var element = Create("<element>", "Line 1\r\nLine 2");

        element.Name.ToString().ShouldBe("element");
        ElementClosure.StartOfElement.ShouldBe(element.Closure);

        var lines = new List<string>();
        element.CopyContentsTo(lines, out _);

        lines.Count.ShouldBe(2);
        lines[0].ShouldBe("Line 1");
        lines[1].ShouldBe("Line 2");
    }

    [Test,
     TestCase("<element attribute1=/>"),
     TestCase("<element attribute1= />"),
     TestCase("<element attribute1=></element>"),
     TestCase("<element attribute1= ></element>"),
     TestCase("<element attribute1= attribute2=''/>"),
     TestCase("<element attribute1=1/>"),
     TestCase("<element attribute1=1 />"),
     TestCase("<element attribute1=1></element>"),
     TestCase("<element attribute1=1 ></element>"),
     TestCase("<element attribute1=1 attribute2=''/>")]
    public void TestElementAttributeNotFound(string @case)
    {
        var element = Create(@case);

        element.Name.ToString().ShouldBe("element");

        var value = element.GetAttribute("attribute");
        value.ShouldBeNull();
    }

    [Test,
     TestCase("<element attribute=/>"),
     TestCase("<element attribute= />"),
     TestCase("<element attribute=></element>"),
     TestCase("<element attribute= ></element>"),
     TestCase("<element attribute= attribute2=''/>")]
    public void TestElementAttributeWithNoValue(string @case)
    {
        var element = Create(@case);

        element.Name.ToString().ShouldBe("element");

        var value = element.GetAttribute("attribute");
        value.ShouldBeEmpty();
    }

    [Test,
     TestCase("<element attribute=1/>", "1"),
     TestCase("<element attribute=1 />", "1"),
     TestCase("<element attribute=1></element>", "1"),
     TestCase("<element attribute=1 ></element>", "1"),
     TestCase("<element attribute=1 attribute2=''/>", "1"),
     TestCase("<element attribute='1/>", "1")]
    public void TestElementAttributeWithNoneQuotedValue(string @case, string? result)
    {
        var element = Create(@case);

        element.Name.ToString().ShouldBe("element");

        var value = element.GetAttribute("attribute");
        value.ShouldBe(result);
    }

    [Test,
     TestCase("<element/>"),
     TestCase("<element></element>")]
    public void TestElementWithNoContent(string @case)
    {
        var element = Create(@case);

        element.Name.ToString().ShouldBe("element");

        var lines = new List<string>();
        element.CopyContentsTo(lines, out _);
        lines.ShouldBeEmpty();
    }

    [Test,
     TestCase(""),
     TestCase("element"),
     TestCase("<element"),
     TestCase("<element "),
     TestCase("element>"),
     TestCase("element>")]
    public void TestInvalidElement(string @case)
    {
        var exception = Should.Throw<InvalidEnumArgumentException>(() => Create(@case));
        exception.Message.ShouldContain(@case);
    }

    [Test,
     TestCase(""),
     TestCase("<element"),
     TestCase("<element ")]
    public void TestNoElements(string @case)
    {
        ElementInfo.Next(@case.AsSpan(), out _).ShouldBeFalse();
    }
}