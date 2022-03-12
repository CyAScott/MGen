using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class IfBuilderTests
{
    [Test]
    public void TestIf()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("int", "Test");
        method.ArgumentParameters.Add("int", "index");

        var @if = method.AddIf("index < 0");
        @if.Return(-1);

        method.Return(0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Test(int index)",
            "        {",
            "            if (index < 0)",
            "            {",
            "                return -1;",
            "            }",
            "            return 0;",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestIfElse()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("int", "Test");
        method.ArgumentParameters.Add("int", "index");

        var @if = method.AddIf("index < 0");
        @if.Return(-1);

        @if.Else.Enabled = true;
        @if.Else.Return(0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Test(int index)",
            "        {",
            "            if (index < 0)",
            "            {",
            "                return -1;",
            "            }",
            "            else",
            "            {",
            "                return 0;",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestIfElseIfElse()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("int", "Test");
        method.ArgumentParameters.Add("int", "index");

        var @if = method.AddIf("index < 0");
        @if.Return(-1);

        @if.ElseIfs.Add("index > 0").Return(1);

        @if.Else.Enabled = true;
        @if.Else.Return(0);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Test(int index)",
            "        {",
            "            if (index < 0)",
            "            {",
            "                return -1;",
            "            }",
            "            else if (index > 0)",
            "            {",
            "                return 1;",
            "            }",
            "            else",
            "            {",
            "                return 0;",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    }
}