using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Events;

class EventDeclarationTests
{
    [Test]
    public void TestEvent() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    event Action Event;",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public event System.Action Event;",
            "    }",
            "}",
            "");

    [Test]
    public void TestEventAttributes() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "public class DescriptionAttribute : Attribute",
            "{",
            "    public DescriptionAttribute(string description) => Description = description;",
            "    public string Description { get; }",
            "}",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    [Description(\"Sample text\")]",
            "    event Action Event;",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        [Example.DescriptionAttribute(\"Sample text\")]",
            "        public event System.Action Event;",
            "    }",
            "}",
            "");

    [Test]
    public void TestEventDescription() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    /// <summary>",
            "    /// Sample text",
            "    /// </summary>",
            "    event Action Event;",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        /// <summary>",
            "        /// Sample text",
            "        /// </summary>",
            "        public event System.Action Event;",
            "    }",
            "}",
            "");
}