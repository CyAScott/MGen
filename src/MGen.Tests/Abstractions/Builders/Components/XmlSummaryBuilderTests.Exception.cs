using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

partial class XmlSummaryBuilderTests
{
    [Test]
    public void TestOneLineException()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Exceptions.Add("T:System.ArgumentException", new()
        {
            "Sample exception text"
        });

        comments.ToCode().ShouldBe(
            "    /// <exception cref=\"T:System.ArgumentException\">Sample exception text</exception>",
            "");
    }

    [Test]
    public void TestOneLineExceptionFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <exception cref=""T:System.ArgumentException"">Sample exception text</exception>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <exception cref=\"T:System.ArgumentException\">Sample exception text</exception>",
            "");
    }

    [Test]
    public void TestMultipleExceptions()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Exceptions.Add("T:System.ArgumentException", new()
        {
            "Sample exception 1 text"
        });
        comments.Exceptions.Add("T:System.ArgumentNullException", new()
        {
            "Sample exception 2 text"
        });

        comments.ToCode().ShouldBe(
            "    /// <exception cref=\"T:System.ArgumentException\">Sample exception 1 text</exception>",
            "    /// <exception cref=\"T:System.ArgumentNullException\">Sample exception 2 text</exception>",
            "");
    }

    [Test]
    public void TestMultipleLineException()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Exceptions.Add("T:System.ArgumentException", new()
        {
            "Sample exception ",
            "text"
        });

        comments.ToCode().ShouldBe(
            "    /// <exception cref=\"T:System.ArgumentException\">",
            "    /// Sample exception ",
            "    /// text",
            "    /// </exception>",
            "");
    }

    [Test]
    public void TestMultipleLineExceptionFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <exception cref=""T:System.ArgumentException"">",
            @"    Sample exception ",
            @"    text",
            @"    </exception>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <exception cref=\"T:System.ArgumentException\">",
            "    /// Sample exception ",
            "    /// text",
            "    /// </exception>",
            "");
    }
}