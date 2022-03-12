using NUnit.Framework;

namespace MGen.Abstractions.Builders;

class InterfaceBuilderTests
{
    [Test]
    public void TestCreateInterface()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddInterface("IExample");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    interface IExample",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateInterfaceWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddInterface("IExample").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    [ExampleAttribute]",
            "    interface IExample",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateInterfaceWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddInterface("IExample").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    /// <summary>",
            "    /// Hello World",
            "    /// </summary>",
            "    interface IExample",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateInterfaceWithGenericParameter()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddInterface("IExample").GenericParameters.Add("T");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    interface IExample<T>",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateInterfaceWithInheritance()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @interface = @namespace.AddInterface("IExample");

        @interface.Inheritance.Add("IInterface");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    interface IExample : IInterface",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateInterfaceWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddInterface("IExample").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    public interface IExample",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateInterfaceWithStaticConstructor()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddInterface("IExample");
        @class.StaticConstructor.Enabled = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    interface IExample",
            "    {",
            "        static IExample()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }
}