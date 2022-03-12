using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
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

        Assert.AreEqual(expectedLines.Length, actualLines.Count);

        for (var index = 0; index < actualLines.Count; index++)
        {
            Assert.AreEqual(expectedLines[index], actualLines[index]);
        }
    }

    [Test]
    public void TestElementWithNoTermination()
    {
        var element = Create("<element>", "Line 1\r\nLine 2");

        Assert.AreEqual("element", element.Name.ToString());
        Assert.AreEqual(ElementClosure.StartOfElement, element.Closure);

        var lines = new List<string>();
        element.CopyContentsTo(lines, out _);

        Assert.AreEqual(2, lines.Count);
        Assert.AreEqual("Line 1", lines[0]);
        Assert.AreEqual("Line 2", lines[1]);
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

        Assert.AreEqual("element", element.Name.ToString());

        var value = element.GetAttribute("attribute");
        Assert.IsNull(value);
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

        Assert.AreEqual("element", element.Name.ToString());

        var value = element.GetAttribute("attribute");
        Assert.IsEmpty(value);
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

        Assert.AreEqual("element", element.Name.ToString());

        var value = element.GetAttribute("attribute");
        Assert.AreEqual(result, value);
    }

    [Test,
     TestCase("<element/>"),
     TestCase("<element></element>")]
    public void TestElementWithNoContent(string @case)
    {
        var element = Create(@case);

        Assert.AreEqual("element", element.Name.ToString());

        var lines = new List<string>();
        element.CopyContentsTo(lines, out _);
        Assert.IsEmpty(lines);
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
        var exception = Assert.Throws<InvalidEnumArgumentException>(() => Create(@case));
        Assert.IsTrue(exception.Message.Contains(@case));
    }

    [Test,
     TestCase(""),
     TestCase("<element"),
     TestCase("<element ")]
    public void TestNoElements(string @case)
    {
        Assert.IsFalse(ElementInfo.Next(@case.AsSpan(), out _));
    }
}