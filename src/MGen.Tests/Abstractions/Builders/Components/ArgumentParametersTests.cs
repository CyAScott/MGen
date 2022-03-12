using System;
using System.Text;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

partial class ArgumentParametersTests
{
    class TestArgumentParametersParent : TestParent, IHaveArgumentParameters
    {
        public TestArgumentParametersParent(int indentLevel = 1)
            : base(indentLevel)
        {
            ArgumentParameters = new(this);
            XmlComments = new(this);
        }

        public ArgumentParameters ArgumentParameters { get; }

        public XmlCommentsBuilder XmlComments { get; }

        public bool ArgumentsEnabled { get; init; } = true;
    }

    [Test]
    public void TestArguments()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent())
        {
            { "object", "arg1" },
            { "int", "arg2" }
        };

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("(object arg1, int arg2)");

        descriptions.ShouldBe("");
    }

    [Test]
    public void TestArgumentWithAttribute()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        parameters.Add("object", "arg").Attributes.Add("NotNull");

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("([NotNull]object arg)");

        descriptions.ShouldBe("");
    }

    [Test]
    public void TestArgumentWithDefaultValue()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        parameters.Add("object?", "arg").DefaultValue = "null";

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("(object? arg = null)");

        descriptions.ShouldBe("");
    }

    [Test]
    public void TestArgumentsDisabled()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent
        {
            ArgumentsEnabled = false
        });

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("");

        descriptions.ShouldBe("");
    }

    [Test]
    public void TestArgumentWithParams()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        parameters.Add("object[]", "arg").IsParams = true;

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("(params object[] arg)");

        descriptions.ShouldBe("");
    }

    [Test,
     TestCase(RefKind.In), TestCase(RefKind.RefReadOnly),
     TestCase(RefKind.None),
     TestCase(RefKind.Out),
     TestCase(RefKind.Ref)]
    public void TestArgumentWithRefKind(RefKind refKind)
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        parameters.Add("object", "arg").RefKind = refKind;

        var (arguments, descriptions) = parameters.ToCode();

        if (refKind == RefKind.None)
        {
            arguments.ShouldBe("(object arg)");
        }
        else
        {
            arguments.ShouldBe($"({refKind.ToString().ToLower()} object arg)");
        }

        descriptions.ShouldBe("");
    }

    [Test]
    public void TestDuplicateArgument()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent())
        {
            { "object", "arg" }
        };

        Assert.Throws<ArgumentException>(() => parameters.Add("object", "arg"));
    }

    [Test]
    public void TestNoArguments()
    {
        var parameters = new ArgumentParameters(new TestArgumentParametersParent());

        var (arguments, descriptions) = parameters.ToCode();

        arguments.ShouldBe("()");

        descriptions.ShouldBe("");
    }
}

static partial class Extensions
{
    public static (string Arguments, string Descriptions) ToCode(this ArgumentParameters parameters)
    {
        var argumentsBuilder = new StringBuilder();
        var descriptionsBuilder = new StringBuilder();

        parameters.AppendArguments(argumentsBuilder);
        parameters.Parent.XmlComments.Generate(descriptionsBuilder);

        return (argumentsBuilder.ToString(), descriptionsBuilder.ToString());
    }
}