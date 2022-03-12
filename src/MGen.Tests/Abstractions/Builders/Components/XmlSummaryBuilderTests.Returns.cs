using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

partial class XmlSummaryBuilderTests
{
    [Test]
    public void TestOneLineReturns()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Returns.Add("Sample return text");

        comments.ToCode().ShouldBe(
            "    /// <returns>Sample return text</returns>",
            "");
    }

    [Test]
    public void TestOneLineReturnsFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <returns>Sample return text</returns>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <returns>Sample return text</returns>",
            "");
    }

    [Test]
    public void TestMultipleLineReturns()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Returns.Add("Sample return ");
        comments.Returns.Add("text");

        comments.ToCode().ShouldBe(
            "    /// <returns>",
            "    /// Sample return ",
            "    /// text",
            "    /// </returns>",
            "");
    }

    [Test]
    public void TestMultipleLineReturnsFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <returns>",
            @"    Sample return ",
            @"    text",
            @"    </returns>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <returns>",
            "    /// Sample return ",
            "    /// text",
            "    /// </returns>",
            "");
    }
}