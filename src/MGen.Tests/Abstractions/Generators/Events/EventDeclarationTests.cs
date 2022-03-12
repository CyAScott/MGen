using NUnit.Framework;

namespace MGen.Abstractions.Generators.Events;

class EventDeclarationTests
{
    [Test]
    public void TestEvent()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    event Action Event;",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "        public event System.Action Event;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestEventAttributes()
    {
        var testModelGenerator = new TestModelGenerator(
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
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "        [Example.DescriptionAttribute(\"Sample text\")]",
            "        public event System.Action Event;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestEventDescription()
    {
        var testModelGenerator = new TestModelGenerator(
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
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
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
}