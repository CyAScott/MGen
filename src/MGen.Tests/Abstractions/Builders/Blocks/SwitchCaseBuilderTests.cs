using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class SwitchCaseBuilderTests
{
    [Test]
    public void TestSwitchCaseWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");
        method.ArgumentParameters.Add("int", "index");

        method.AddSwitchCase("index").Enabled = false;

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(int index)",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSwitchCaseWithNoCases()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");
        method.ArgumentParameters.Add("int", "index");

        method.AddSwitchCase("index");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(int index)",
            "        {",
            "            switch (index)",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSwitchCaseWithCase()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");
        method.ArgumentParameters.Add("int", "index");

        var @switch = method.AddSwitchCase("index");

        var @case = @switch.Cases.Add(0);

        @case.AddLine("System.Console.WriteLine(-1)");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(int index)",
            "        {",
            "            switch (index)",
            "            {",
            "                case 0:",
            "                    {",
            "                        System.Console.WriteLine(-1);",
            "                    }",
            "                    break;",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSwitchCaseWithDefaultCase()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");
        method.ArgumentParameters.Add("int", "index");

        var @switch = method.AddSwitchCase("index");

        var @case = @switch.Cases.AddDefault();

        @case.AddLine("System.Console.WriteLine(-1)");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(int index)",
            "        {",
            "            switch (index)",
            "            {",
            "                default:",
            "                    {",
            "                        System.Console.WriteLine(-1);",
            "                    }",
            "                    break;",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}