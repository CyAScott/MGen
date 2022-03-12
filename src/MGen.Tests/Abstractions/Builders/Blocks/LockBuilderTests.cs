using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class LockBuilderTests
{
    [Test]
    public void TestLock()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");
        method.ArgumentParameters.Add("object", "obj");

        method.AddLock("obj");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(object obj)",
            "        {",
            "            lock (obj)",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}