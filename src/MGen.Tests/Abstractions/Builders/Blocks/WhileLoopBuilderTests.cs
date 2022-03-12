using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class WhileLoopBuilderTests
{
    [Test]
    public void TestWhileLoop()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddWhileLoop("true")
            .AddLine("System.Console.WriteLine()");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            while (true)",
            "            {",
            "                System.Console.WriteLine();",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}