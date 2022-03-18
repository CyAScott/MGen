using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Events;

class DuplicateEventTests
{
    [Test]
    public void TestDifferentType() =>
        Compile(
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
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
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

    [Test]
    public void TestDifferentTypeAndOrderOfInterfaces() =>
        Compile(
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
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
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

    [Test]
    public void TestSameType() =>
        Compile(
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
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public event System.Action<int>? Event;",
            "    }",
            "}",
            "");
}