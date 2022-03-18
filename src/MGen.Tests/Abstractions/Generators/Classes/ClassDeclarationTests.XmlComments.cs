using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test]
    public void TestClassDeclarationWithXmlComments() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "/// <summary>",
            "/// Line 1",
            "/// Line 2",
            "/// </summary>",
            "[Generate]",
            "interface IExample { }")
        .ShouldBe(
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