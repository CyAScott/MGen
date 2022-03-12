using MGen.Abstractions.Builders.Members;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Blocks;

class TryCatchBuilderTests
{
    [Test]
    public void TestTryCatch()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddTryCatch("System.Exception", "exception");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            try",
            "            {",
            "            }",
            "            catch (System.Exception exception)",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestTryCatchWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddTryCatch("System.Exception", "exception").Enabled = false;
        
        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestTryCatchWithFinally()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddTryCatch("System.Exception", "exception").Finally.Enabled = true;

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            try",
            "            {",
            "            }",
            "            catch (System.Exception exception)",
            "            {",
            "            }",
            "            finally",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestTryCatchAndWhen()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var method = @class.AddMethod("void", "Test");

        method.AddTryCatch("System.Exception", "exception", "exception.Message == \"\"");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test()",
            "        {",
            "            try",
            "            {",
            "            }",
            "            catch (System.Exception exception) when (exception.Message == \"\")",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }
}