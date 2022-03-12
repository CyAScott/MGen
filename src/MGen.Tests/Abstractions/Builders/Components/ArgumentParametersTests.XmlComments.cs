using System;
using NUnit.Framework;
using TestXmlCommentsParent = MGen.Abstractions.Builders.Components.XmlSummaryBuilderTests.TestXmlCommentsParent;

namespace MGen.Abstractions.Builders.Components;

partial class ArgumentParametersTests
{
    [Test]
    public void TestOneLineParameter()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        parameters.Add("object", "arg").Description.Add("Sample parameter text");

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("(object arg)");

        descriptions.ShouldBe("    /// <param name=\"arg\">Sample parameter text</param>" + Environment.NewLine);
    }

    [Test]
    public void TestOneLineParameterFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <param name=""arg"">Sample parameter text</param>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <param name=\"arg\">Sample parameter text</param>",
            "");
    }

    [Test]
    public void TestMultipleParameters()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        parameters.Add("object", "arg1").Description.Add("Sample parameter 1 text");
        parameters.Add("object", "arg2").Description.Add("Sample parameter 2 text");

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("(object arg1, object arg2)");

        descriptions.ShouldBe(
            "    /// <param name=\"arg1\">Sample parameter 1 text</param>",
            "    /// <param name=\"arg2\">Sample parameter 2 text</param>",
            "");
    }

    [Test]
    public void TestMultipleLineParameter()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        var description = parameters.Add("object", "arg").Description;
        description.Add("Sample parameter ");
        description.Add("text");

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("(object arg)");

        descriptions.ShouldBe(
            "    /// <param name=\"arg\">",
            "    /// Sample parameter ",
            "    /// text",
            "    /// </param>",
            "");
    }

    [Test]
    public void TestMultipleLineParameterFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <param name=""arg"">",
            @"    Sample parameter ",
            @"    text",
            @"    </param>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <param name=\"arg\">",
            "    /// Sample parameter ",
            "    /// text",
            "    /// </param>",
            "");
    }
}