using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class UsingBlockBuilderTests
{
    [Test]
    public void TestUsingBlock()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");
        method.ArgumentParameters.Add("System.IDisposable", "obj");

        method.AddUsingBlock("obj");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(System.IDisposable obj)",
            "        {",
            "            using (obj)",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}