using NUnit.Framework;

namespace MGen.Abstractions.Builders.Members;

class EventBuilderTests
{
    [Test]
    public void TestCreateEvent()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddEvent("Action", "Event");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        event Action Event;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateEventWithAttribute()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddEvent("Action", "Event").Attributes.Add("ExampleAttribute");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        [ExampleAttribute]",
            "        event Action Event;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateEventWithDescription()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddEvent("Action", "Event").XmlComments.Add("Hello World");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        /// <summary>",
            "        /// Hello World",
            "        /// </summary>",
            "        event Action Event;",
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
        @class.AddEvent("Action", "Event").ExplicitDeclaration.SetExplicitDeclaration("IInterface");

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example : IInterface",
            "    {",
            "        event Action IInterface.Event",
            "        {",
            "            add => throw new System.NotImplementedException();",
            "            remove => throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestCreateEventWithDisabled()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddEvent("Action", "Event").Enabled = false;

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
    public void TestCreateEventWithModifier()
    {
        var @namespace = new NamespaceBuilder("Test");

        var @class = @namespace.AddClass("Example");

        @class.AddEvent("Action", "Event").Modifiers.IsPublic = true;

        var code = @namespace.ToCode();

        code.ShouldBe(
            "namespace Test",
            "{",
            "    class Example",
            "    {",
            "        public event Action Event;",
            "    }",
            "}",
            "");
    }
}