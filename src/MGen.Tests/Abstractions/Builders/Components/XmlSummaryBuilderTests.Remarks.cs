using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

partial class XmlSummaryBuilderTests
{
    [Test]
    public void TestOneLineRemarks()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Remarks.Add("Sample remarks text");

        comments.ToCode().ShouldBe(
            "    /// <remarks>Sample remarks text</remarks>",
            "");
    }

    [Test]
    public void TestOneLineRemarksFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <remarks>Sample remarks text</remarks>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <remarks>Sample remarks text</remarks>",
            "");
    }

    [Test]
    public void TestMultipleLineRemarks()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Remarks.Add("Sample remarks ");
        comments.Remarks.Add("text");

        comments.ToCode().ShouldBe(
            "    /// <remarks>",
            "    /// Sample remarks ",
            "    /// text",
            "    /// </remarks>",
            "");
    }

    [Test]
    public void TestMultipleLineRemarksFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <remarks>",
            @"    Sample remarks ",
            @"    text",
            @"    </remarks>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <remarks>",
            "    /// Sample remarks ",
            "    /// text",
            "    /// </remarks>",
            "");
    }
}