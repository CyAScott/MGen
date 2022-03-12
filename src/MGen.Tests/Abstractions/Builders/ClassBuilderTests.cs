using NUnit.Framework;

namespace MGen.Abstractions.Builders;

class ClassBuilderTests
{
    [Test]
    public void TestCreateClass()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddClass("Example");

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
    public void TestCreateClassWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddClass("Example").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    [ExampleAttribute]",
            "    class Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateClassWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddClass("Example").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    /// <summary>",
            "    /// Hello World",
            "    /// </summary>",
            "    class Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateClassWithGenericParameter()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddClass("Example").GenericParameters.Add("T");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example<T>",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateClassWithInheritance()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.Inheritance.Add("IInterface");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example : IInterface",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateClassWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddClass("Example").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    public class Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateClassWithStaticConstructor()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");
        @class.Modifiers.IsStatic = true;
        @class.StaticConstructor.Enabled = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    static class Example",
            "    {",
            "        static Example()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }
}