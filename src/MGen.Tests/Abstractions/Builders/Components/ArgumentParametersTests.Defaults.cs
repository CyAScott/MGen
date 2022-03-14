using MGen.Abstractions.Generators;
using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Builders.Components;

partial class ArgumentParametersTests
{
    [Test]
    public void TestDefaultBooleanValue()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(bool disabled = false);",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get(bool disabled = false)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestDefaultNullValue()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(string? keyword = null);",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get(string? keyword = null)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestDefaultPrimitiveValue()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(int count = 10);",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get(int count = 10)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestDefaultStringValue()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(string keyword = \"\");",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get(string keyword = \"\")",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }
}