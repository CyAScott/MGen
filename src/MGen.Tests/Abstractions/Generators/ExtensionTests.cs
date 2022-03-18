using System.Linq;
using MGen.Abstractions.Generators.Extensions;
using Microsoft.CodeAnalysis;
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

        testModelGenerator.Compile(typeof(Demo).Assembly)
            .EmitResult
            .Diagnostics
            .Where(it => it.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();

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