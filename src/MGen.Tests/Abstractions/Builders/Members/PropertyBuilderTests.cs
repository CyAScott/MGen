using MGen.Abstractions.Builders.Blocks;
using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

partial class PropertyBuilderTests
{
    [Test]
    public void TestCreateProperty()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(property.Field.Name);
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property",
            "        {",
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
    public void TestCreatePropertyWithArguments()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddIndexProperty("object?", "int", "index");
        property.Get.Return(Code.Null);
        property.Set.AddLine("throw System.IndexOutOfRangeException()");
        
        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        object? this[int index]",
            "        {",
            "            get",
            "            {",
            "                return null;",
            "            }",
            "            set",
            "            {",
            "                throw System.IndexOutOfRangeException();",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreatePropertyWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Attributes.Add("ExampleAttribute");
        property.Get.Return(property.Field.Name);
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        [ExampleAttribute]",
            "        int Property",
            "        {",
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
    public void TestCreatePropertyWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(property.Field.Name);
        property.Set.Set(property.Field.Name, "value");
        property.XmlComments.Add("Hello World");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        /// <summary>",
            "        /// Hello World",
            "        /// </summary>",
            "        int Property",
            "        {",
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
    public void TestCreatePropertyWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Enabled = false;
        property.Get.Return(property.Field.Name);
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "    }",
            "}",
            "");
    }
    
    [Test]
    public void TestCreatePropertyWithExplicitDeclaration()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");
        @class.Inheritance.Add("IInterface");

        var property = @class.AddProperty("int", "Property");
        property.ExplicitDeclaration.SetExplicitDeclaration("IInterface");
        property.Get.Return(property.Field.Name);
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example : IInterface",
            "    {",
            "        int IInterface.Property",
            "        {",
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
    public void TestCreatePropertyWithInitializer()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(property.Field.Name);
        property.Initializer = 0;
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property",
            "        {",
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
            "        int _property = 0;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreatePropertyWithInitializerNoField()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(property.Field.Name);
        property.Initializer = 0;
        property.Set.Set(property.Field.Name, "value");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        int Property",
            "        {",
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
            "        int _property = 0;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreatePropertyWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        var property = @class.AddProperty("int", "Property");
        property.Get.Return(property.Field.Name);
        property.Modifiers.IsPublic = true;
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
}