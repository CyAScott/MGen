using MGen.Abstractions.Generators.Extensions;
using NUnit.Framework;
using Shouldly;

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
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
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