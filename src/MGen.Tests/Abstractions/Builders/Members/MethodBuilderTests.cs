using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

class MethodBuilderTests
{
    [Test]
    public void TestCreateMethod()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test");

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
    public void TestCreateMethodWithArgumentParameters()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").ArgumentParameters.Add("int", "value");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test(int value)",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateMethodWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        [ExampleAttribute]",
            "        void Test()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateMethodWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        /// <summary>",
            "        /// Hello World",
            "        /// </summary>",
            "        void Test()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateMethodWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").Enabled = false;

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
    public void TestCreateEventWithExplicitDeclaration()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.Inheritance.Add("IInterface");

        @class.AddMethod("void", "Test").ExplicitDeclaration.SetExplicitDeclaration("IInterface");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example : IInterface",
            "    {",
            "        void IInterface.Test()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateMethodWithGenericParameters()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").GenericParameters.Add("TValue");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test<TValue>()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateMethodWithGenericParametersWithConstraint()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").GenericParameters.Add("TValue", "new()");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        void Test<TValue>()",
            "            where TValue : new()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateMethodWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddMethod("void", "Test").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        public void Test()",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }
}