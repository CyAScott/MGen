using MGen.Abstractions.Builders.Members;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class LineBuilderTests
{
    [Test]
    public void TestAddLine()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test")
            .AddLine("System.Console.WriteLine()");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            System.Console.WriteLine();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestReturn()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test")
            .Return();

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            return;",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestReturnWithValue()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("int", "Test")
            .Return(0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Test()",
            "        {",
            "            return 0;",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSet()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.ArgumentParameters.Add("int", "index").RefKind = RefKind.Out;
        method.Set("index", 0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(out int index)",
            "        {",
            "            index = 0;",
            "        }",
            "    }",
            "}",
            "");
    }
}