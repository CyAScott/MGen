using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

class ConstructorBuilderTests
{
    [Test]
    public void TestCreateConstructor()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddConstructor();

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        Example()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithArgumentParameters()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddConstructor().ArgumentParameters.Add("int", "value");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        Example(int value)",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddConstructor().Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        [ExampleAttribute]",
            "        Example()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddConstructor().XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        /// <summary>",
            "        /// Hello World",
            "        /// </summary>",
            "        Example()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddConstructor().Enabled = false;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddConstructor().Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        public Example()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }
}