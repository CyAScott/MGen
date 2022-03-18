using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test,
     TestCase(""),
     TestCase("true"),
     TestCase("false"),
     TestCase("1"),
     TestCase("\"Test String\""),
     TestCase("(Example.TestEnum)2"),
     TestCase("B = true"),
     TestCase("B = false"),
     TestCase("I = 1"),
     TestCase("S = \"Test String\""),
     TestCase("E = (Example.TestEnum)2"),
     TestCase("\"Test String\", B = true"),
     TestCase("\"Test String\", B = false"),
     TestCase("\"Test String\", I = 1"),
     TestCase("(Example.TestEnum)2, S = \"Test String\""),
     TestCase("\"Test String\", E = (Example.TestEnum)2")]
    public void TestClassDeclarationWithAttributes(string attributeParameters) =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public enum TestEnum",
            "{",
            "    One = 1,",
            "    Two = 2,",
            "    Three = 3",
            "}",
            "",
            "public class TestAttribute : System.Attribute",
            "{",
            "    public TestAttribute()",
            "    {",
            "    }",
            "    public TestAttribute(bool b)",
            "    {",
            "    }",
            "    public TestAttribute(int i)",
            "    {",
            "    }",
            "    public TestAttribute(string? s)",
            "    {",
            "    }",
            "    public TestAttribute(TestEnum e)",
            "    {",
            "    }",
            "    ",
            "    public bool B { get; set; }",
            "    public int I { get; set; }",
            "    public string? S { get; set; }",
            "    public TestEnum E { get; set; } = TestEnum.One;",
            "}",
            "",
            $"[{(string.IsNullOrEmpty(attributeParameters) ? "Test" : $"Test({attributeParameters})")}, Generate]",
            "interface IExample { }")
        .ShouldBe(
            "namespace Example",
            "{",
            $"    [{(string.IsNullOrEmpty(attributeParameters) ? "Example.TestAttribute" : $"Example.TestAttribute({ attributeParameters})")}]",
            "    class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");
}