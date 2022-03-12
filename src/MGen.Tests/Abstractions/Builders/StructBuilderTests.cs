using NUnit.Framework;

namespace MGen.Abstractions.Builders;

class StructBuilderTests
{
    [Test]
    public void TestCreateStruct()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddStruct("Example");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    struct Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateStructWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddStruct("Example").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    [ExampleAttribute]",
            "    struct Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateStructWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddStruct("Example").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    /// <summary>",
            "    /// Hello World",
            "    /// </summary>",
            "    struct Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateStructWithGenericParameter()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddStruct("Example").GenericParameters.Add("T");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    struct Example<T>",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateStructWithInheritance()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddStruct("Example");

        @class.Inheritance.Add("IInterface");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    struct Example : IInterface",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateStructWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddStruct("Example").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    public struct Example",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateStructWithStaticConstructor()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @struct = @namespace.AddStruct("Example");
        @struct.StaticConstructor.Enabled = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    struct Example",
            "    {",
            "        static Example()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }
}