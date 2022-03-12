using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class ForLoopBuilderTests
{
    [Test]
    public void TestForLoop()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        var @for = method.AddForLoop("var index", "index < 10", "index++");

        @for.AddLine("System.Console.WriteLine(index)");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            for (var index; index < 10; index++)",
            "            {",
            "                System.Console.WriteLine(index);",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}