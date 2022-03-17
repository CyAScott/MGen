using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Generators.Methods;

class DuplicateMethodTests
{
    [Test]
    public void TestDifferentSignatureAndDifferentReturnType()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Set(int value);",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    long Set(long value);",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveLongMethod",
            "{",
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
            "        public int Set(int value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        public long Set(long value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestDifferentSignatureAndSameReturnType()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    void Set(int value);",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    void Set(long value);",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveLongMethod",
            "{",
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
            "        public void Set(int value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        public void Set(long value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSameSignatureAndDifferentReturnType()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Get();",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    long Get();",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveLongMethod",
            "{",
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
            "        public int Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        long Example.IHaveLongMethod.Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSameSignatureAndDifferentReturnTypeAndOrderOfInterfaces()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Get();",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    long Get();",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveLongMethod, IHaveIntMethod",
            "{",
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
            "        public long Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        int Example.IHaveIntMethod.Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSameSignatureAndSameReturnType()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Get();",
            "}",
            "",
            "interface IHaveIntMethodToo",
            "{",
            "    int Get();",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveIntMethodToo",
            "{",
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
            "        public int Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }
}