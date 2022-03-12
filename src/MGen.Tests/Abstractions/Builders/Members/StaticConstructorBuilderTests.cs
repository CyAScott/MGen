using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

class StaticConstructorBuilderTests
{
    [Test]
    public void TestCreateConstructor()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.StaticConstructor.Enabled = true;

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        static Example()",
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

        @class.StaticConstructor.Enabled = true;

        @class.StaticConstructor.Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        [ExampleAttribute]",
            "        static Example()",
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
}