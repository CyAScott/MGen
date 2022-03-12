using MGen.Abstractions.Generators.Extensions;
using NUnit.Framework;

namespace MGen.Abstractions.Generators;

class ExtensionTests
{
    [Test]
    public void TestExtension()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(new [] { typeof(Demo).Assembly }, out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "        int Get()",
            "        {",
            "            return 0;",
            "        }",
            "    }",
            "}",
            "");
    }
}