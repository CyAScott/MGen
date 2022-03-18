using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Builders.Components;

partial class ArgumentParametersTests
{
    [Test]
    public void TestDefaultBooleanValue() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(bool disabled = false);",
            "}")
        .ShouldBe(
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

    [Test]
    public void TestDefaultNullValue() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(string? keyword = null);",
            "}")
        .ShouldBe(
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

    [Test]
    public void TestDefaultPrimitiveValue() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(int count = 10);",
            "}")
        .ShouldBe(
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

    [Test]
    public void TestDefaultStringValue() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get(string keyword = \"\");",
            "}")
        .ShouldBe(
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