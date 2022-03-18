using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test,
     TestCase("public"),
     TestCase("public partial"),
     TestCase("internal"),
     TestCase("internal partial"),
     TestCase("partial")]
    public void TestClassDeclarationWithModifiers(string modifiers) =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            $"{modifiers} interface IExample {{ }}")
        .ShouldBe(
            "namespace Example",
            "{",
            $"    {modifiers} class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");
}