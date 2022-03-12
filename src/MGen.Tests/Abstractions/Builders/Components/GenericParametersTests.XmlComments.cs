using System;
using NUnit.Framework;
using TestXmlCommentsParent = MGen.Abstractions.Builders.Components.XmlSummaryBuilderTests.TestXmlCommentsParent;

namespace MGen.Abstractions.Builders.Components;

partial class GenericParametersTests
{
    [Test]
    public void TestOneLineParameter()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        parameters.Add("TExample").Description.Add("Sample parameter text");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample>");
        constraints.ShouldBe();
        descriptions.ShouldBe("    /// <typeparam name=\"TExample\">Sample parameter text</typeparam>" + Environment.NewLine);
    }

    [Test]
    public void TestOneLineParameterFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <typeparam name=""TExample"">Sample parameter text</typeparam>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <typeparam name=\"TExample\">Sample parameter text</typeparam>",
            "");
    }

    [Test]
    public void TestMultipleParameters()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        parameters.Add("TExample1").Description.Add("Sample parameter 1 text");
        parameters.Add("TExample2").Description.Add("Sample parameter 2 text");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample1, TExample2>");
        constraints.ShouldBe();
        descriptions.ShouldBe(
            "    /// <typeparam name=\"TExample1\">Sample parameter 1 text</typeparam>",
            "    /// <typeparam name=\"TExample2\">Sample parameter 2 text</typeparam>",
            "");
    }

    [Test]
    public void TestMultipleLineParameter()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        var description = parameters.Add("TExample").Description;
        description.Add("Sample parameter ");
        description.Add("text");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample>");
        constraints.ShouldBe();
        descriptions.ShouldBe(
            "    /// <typeparam name=\"TExample\">",
            "    /// Sample parameter ",
            "    /// text",
            "    /// </typeparam>",
            "");
    }

    [Test]
    public void TestMultipleLineParameterFromSymbol()
    {
        var comments = new TestXmlCommentsParent(
            @"<member name=""M:Example.IExample.Method"">",
            @"    <typeparam name=""TExample"">",
            @"    Sample parameter ",
            @"    text",
            @"    </typeparam>",
            @"</member>",
            "").XmlComments;

        comments.ToCode().ShouldBe(
            "    /// <typeparam name=\"TExample\">",
            "    /// Sample parameter ",
            "    /// text",
            "    /// </typeparam>",
            "");
    }
}