using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

class DelegateBuilderTests
{
    [Test]
    public void TestCreateDelegate()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    delegate void Test();",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithArgumentParameters()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").ArgumentParameters.Add("int", "value");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    delegate void Test(int value);",
            "}",
            "");
    }

    [Test]
    public void TestCreateDelegateWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    [ExampleAttribute]",
            "    delegate void Test();",
            "}",
            "");
    }

    [Test]
    public void TestCreateDelegateWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    /// <summary>",
            "    /// Hello World",
            "    /// </summary>",
            "    delegate void Test();",
            "}",
            "");
    }

    [Test]
    public void TestCreateDelegateWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").Enabled = false;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithGenericParameters()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").GenericParameters.Add("TValue");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    delegate void Test<TValue>();",
            "}",
            "");
    }

    [Test]
    public void TestCreateConstructorWithGenericParametersWithConstraint()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").GenericParameters.Add("TValue", "new()");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    delegate void Test<TValue>()",
            "        where TValue : new();",
            "}",
            "");
    }

    [Test]
    public void TestCreateDelegateWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddDelegate("void", "Test").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    public delegate void Test();",
            "}",
            "");
    }
}