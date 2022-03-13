using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Generators.Events;

class DuplicateEventTests
{
    [Test]
    public void TestDifferentType()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntEvent",
            "{",
            "    event Action<int>? Event;",
            "}",
            "",
            "interface IHaveLongEvent",
            "{",
            "    event Action<long>? Event;",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntEvent, IHaveLongEvent",
            "{",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "        public event System.Action<int>? Event;",
            "",
            "        event System.Action<long>? Example.IHaveLongEvent.Event",
            "        {",
            "            add => throw new System.NotImplementedException();",
            "            remove => throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestDifferentTypeAndOrderOfInterfaces()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntEvent",
            "{",
            "    event Action<int>? Event;",
            "}",
            "",
            "interface IHaveLongEvent",
            "{",
            "    event Action<long>? Event;",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveLongEvent, IHaveIntEvent",
            "{",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "        public event System.Action<long>? Event;",
            "",
            "        event System.Action<int>? Example.IHaveIntEvent.Event",
            "        {",
            "            add => throw new System.NotImplementedException();",
            "            remove => throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSameType()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntEvent",
            "{",
            "    event Action<int>? Event;",
            "}",
            "",
            "interface IHaveIntEventToo",
            "{",
            "    event Action<int>? Event;",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntEvent, IHaveIntEventToo",
            "{",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "        public event System.Action<int>? Event;",
            "    }",
            "}",
            "");
    }
}