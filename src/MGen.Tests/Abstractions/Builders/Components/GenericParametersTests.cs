using System.Text;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

partial class GenericParametersTests
{
    class TestGenericParametersParent : TestParent, IHaveGenericParameters
    {
        public TestGenericParametersParent(int indentLevel = 1)
            : base(indentLevel)
        {
            GenericParameters = new(this);
            XmlComments = new(this);
        }

        public GenericParameters GenericParameters { get; }
        public XmlCommentsBuilder XmlComments { get; }
    }

    [Test]
    public void TestParametersWithAttribute()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        parameters.Add("TExample1").Attributes.Add("Example1");
        parameters.Add("TExample2").Attributes.Add("Example2");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<[Example1]TExample1, [Example2]TExample2>");
        constraints.ShouldBe();
        descriptions.ShouldBe();
    }

    [Test]
    public void TestParametersWithConstraints()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent())
        {
            { "TExample1", "new()" },
            { "TExample2", "class" }
        };

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample1, TExample2>");
        constraints.ShouldBe(
            "        where TExample1 : new()",
            "        where TExample2 : class");
        descriptions.ShouldBe();
    }

    [Test]
    public void TestParametersWithDescription()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        parameters.Add("TExample1").Description.Add("Hello World 1");
        parameters.Add("TExample2").Description.Add("Hello World 2");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample1, TExample2>");
        constraints.ShouldBe();
        descriptions.ShouldBe(
            "    /// <typeparam name=\"TExample1\">Hello World 1</typeparam>",
            "    /// <typeparam name=\"TExample2\">Hello World 2</typeparam>",
            "");
    }

    [Test]
    public void TestParameterWithAttribute()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        parameters.Add("TExample").Attributes.Add("Example");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<[Example]TExample>");
        constraints.ShouldBe();
        descriptions.ShouldBe();
    }

    [Test]
    public void TestParameterWithConstraints()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent())
        {
            { "TExample", "new()" }
        };

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample>");
        constraints.ShouldBe("        where TExample : new()");
        descriptions.ShouldBe();
    }

    [Test]
    public void TestParameterWithDescription()
    {
        var parameters = new GenericParameters(new TestGenericParametersParent());

        parameters.Add("TExample").Description.Add("Hello World");

        var (@params, constraints, descriptions) = parameters.ToCode();

        @params.ShouldBe("<TExample>");
        constraints.ShouldBe();
        descriptions.ShouldBe("    /// <typeparam name=\"TExample\">Hello World</typeparam>", "");
    }
}

static partial class Extensions
{
    public static (string Parameters, string Constraints, string Descriptions) ToCode(this GenericParameters parameters)
    {
        var parametersBuilder = new StringBuilder();
        var constraintsBuilder = new StringBuilder();
        var descriptionsBuilder = new StringBuilder();

        parameters.AppendParameters(parametersBuilder);
        parameters.AppendConstraints(constraintsBuilder);
        parameters.Parent.XmlComments.Generate(descriptionsBuilder);

        return (parametersBuilder.ToString(), constraintsBuilder.ToString(), descriptionsBuilder.ToString());
    }
}