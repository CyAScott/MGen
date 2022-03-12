using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

class FieldBuilderTests
{
    [Test]
    public void TestCreateField()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddField("int", "Field");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Field;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateFieldWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddField("int", "Field").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        [ExampleAttribute]",
            "        int Field;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateFieldWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddField("int", "Field").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        /// <summary>",
            "        /// Hello World",
            "        /// </summary>",
            "        int Field;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateFieldWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddField("int", "Field").Enabled = false;

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
    public void TestCreateFieldWithInitializer()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddField("int", "Field").Initializer = 0;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Field = 0;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateFieldWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddField("int", "Field").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        public int Field;",
            "    }",
            "}",
            "");
    }
}