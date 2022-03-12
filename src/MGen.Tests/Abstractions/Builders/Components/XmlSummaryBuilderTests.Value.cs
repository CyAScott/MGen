using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

partial class XmlSummaryBuilderTests
{
    [Test]
    public void TestOneLineValue()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Value.Add("Sample value text");

        comments.ToCode().ShouldBe(
            "    /// <value>Sample value text</value>",
            "");
    }

    [Test]
    public void TestOneLineValueFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <value>Sample value text</value>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <value>Sample value text</value>",
            "");
    }

    [Test]
    public void TestMultipleLineValue()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Value.Add("Sample value ");
        comments.Value.Add("text");

        comments.ToCode().ShouldBe(
            "    /// <value>",
            "    /// Sample value ",
            "    /// text",
            "    /// </value>",
            "");
    }

    [Test]
    public void TestMultipleLineValueFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <value>",
            @"    Sample value ",
            @"    text",
            @"    </value>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <value>",
            "    /// Sample value ",
            "    /// text",
            "    /// </value>",
            "");
    }
}