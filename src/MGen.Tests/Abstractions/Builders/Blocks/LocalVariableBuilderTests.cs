using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class LocalVariableBuilderTests
{
    [Test]
    public void TestLocalVariable()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddVariable("int", "index", 0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            int index = 0;",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestLocalVarVariable()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddVariable("var", "index", 0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            var index = 0;",
            "        }",
            "    }",
            "}",
            "");
    }
    [Test]
    public void TestLocalVariableWithoutInitializer()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddVariable("int", "index");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            int index;",
            "        }",
            "    }",
            "}",
            "");
    }
}