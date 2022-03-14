using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test]
    public void TestClassDeclarationWithXmlComments()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "/// <summary>",
            "/// Line 1",
            "/// Line 2",
            "/// </summary>",
            "[Generate]",
            "interface IExample { }");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    /// <summary>",
            "    /// Line 1",
            "    /// Line 2",
            "    /// </summary>",
            "    class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");
    }
}