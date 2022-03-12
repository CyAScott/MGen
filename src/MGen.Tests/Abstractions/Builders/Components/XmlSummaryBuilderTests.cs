using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

internal partial class XmlSummaryBuilderTests
{
    [DebuggerStepThrough]
    public class TestXmlCommentsParent : TestParent, IHaveXmlComments
    {
        public TestXmlCommentsParent(params string[] comments)
        {
            if (comments.Length == 0)
            {
                XmlComments = new(this);
            }
            else
            {
                var mock = new Mock<ISymbol>();

                mock.Setup(it => it.GetDocumentationCommentXml(
                        It.IsAny<CultureInfo?>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(string.Join(Environment.NewLine, comments));

                XmlComments = new(this, mock.Object);
            }
        }

        public XmlCommentsBuilder XmlComments { get; }
    }

    [Test]
    public void TestNoSummary()
    {
        var comments = new TestXmlCommentsParent().XmlComments;

        comments.ToCode().ShouldBe("");
    }

    [Test]
    public void TestOneLineSummary()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Add("Hello World");

        comments.ToCode().ShouldBe(
            "    /// <summary>",
            "    /// Hello World",
            "    /// </summary>",
            "");
    }

    [Test]
    public void TestOneLineSummaryFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""T:Example.IExample"">",
            @"    <summary>",
            @"    Hello World",
            @"    </summary>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <summary>",
            "    /// Hello World",
            "    /// </summary>",
            "");
    }

    [Test]
    public void TestMultipleLineSummary()
    {
        var comments = new TestXmlCommentsParent().XmlComments;
        comments.Add("Hello");
        comments.Add("World");

        comments.ToCode().ShouldBe(
            "    /// <summary>",
            "    /// Hello",
            "    /// World",
            "    /// </summary>",
            "");
    }

    [Test]
    public void TestMultipleLineSummaryFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""T:Example.IExample"">",
            @"    <summary>",
            @"    Hello ",
            @"    World",
            @"    </summary>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <summary>",
            "    /// Hello ",
            "    /// World",
            "    /// </summary>",
            "");
    }
}