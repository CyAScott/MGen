using MGen.Abstractions.Builders.Blocks;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

partial class PropertyBuilderTests
{
    [Test]
    public void TestCreateGetterSetterWithAbstractProperty()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");
        @class.Modifiers.IsAbstract = true;

        var property = @class.AddProperty("int", "Property", false);
        property.Modifiers.IsAbstract = true;

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    abstract class Example",
            "    {",
            "        abstract int Property { get; set; }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Attributes.Add("ExampleAttribute");
        property.Get.Return(property.Field.Name);
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property",
            "        {",
            "            [ExampleAttribute]",
            "            get",
            "            {",
            "                return _property;",
            "            }",
            "            set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        int _property;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterWithAttributeAndNoBody()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property", false);
        property.Get.Attributes.Add("ExampleAttribute");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property { [ExampleAttribute] get; set; }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Enabled = false;
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property",
            "        {",
            "            set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        int _property;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterWithModifiers()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(property.Field.Name);
        property.Modifiers.IsPublic = true;
        property.Set.Modifiers.IsPrivate = true;
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        public int Property",
            "        {",
            "            get",
            "            {",
            "                return _property;",
            "            }",
            "            private set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        int _property;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterWithNoBody()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddProperty("int", "Property", false);

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property { get; set; }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterInInterfacePropertyWithModifiers()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddInterface("IInterface");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(0);
        property.Modifiers.IsPublic = true;
        property.Set.Enabled = false;

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    interface IInterface",
            "    {",
            "        public int Property",
            "        {",
            "            get",
            "            {",
            "                return 0;",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateGetterInInterfacePropertyWithNoModifiers()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddInterface("IInterface");

        var property = @class.AddProperty("int", "Property");
        //even though there is a body for the get, because the interface member for the property can't have a body
        property.Get.Return(0);
        property.Set.Enabled = false;

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    interface IInterface",
            "    {",
            "        int Property { get; }",
            "    }",
            "}",
            "");
    }
}