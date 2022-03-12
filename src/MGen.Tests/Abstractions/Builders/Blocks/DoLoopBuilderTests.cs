using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class DoLoopBuilderTests
{
    [Test]
    public void TestDoLoop()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddDoLoop("true")
            .AddLine("System.Console.WriteLine()");
        
        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            do",
            "            {",
            "                System.Console.WriteLine();",
            "            } while (true);",
            "        }",
            "    }",
            "}",
            "");
    }
}