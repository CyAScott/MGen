using NUnit.Framework;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test,
     TestCase("public"),
     TestCase("public partial"),
     TestCase("internal"),
     TestCase("internal partial"),
     TestCase("partial")]
    public void TestClassDeclarationWithModifiers(string modifiers)
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            $"{modifiers} interface IExample {{ }}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            $"    {modifiers} class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");
    }
}