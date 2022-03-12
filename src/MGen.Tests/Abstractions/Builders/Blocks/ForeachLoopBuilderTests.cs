using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class ForeachLoopBuilderTests
{
    [Test]
    public void TestForeachLoop()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        var @foreach = method.AddForeachLoop("int", "index", "System.Linq.Enumerable.Range(0, 10)");

        @foreach.AddLine("System.Console.WriteLine(index)");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            foreach (int index in System.Linq.Enumerable.Range(0, 10))",
            "            {",
            "                System.Console.WriteLine(index);",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestForeachVarLoop()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        var @foreach = method.AddForeachLoop("index", "System.Linq.Enumerable.Range(0, 10)");

        @foreach.AddLine("System.Console.WriteLine(index)");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            foreach (var index in System.Linq.Enumerable.Range(0, 10))",
            "            {",
            "                System.Console.WriteLine(index);",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}