using NUnit.Framework;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test]
    public void TestClassDeclarationWithGenerics()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<T> { }");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel<T> : IExample<T>",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationWithGenericsAndConstraints()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<TKey, TValue>",
            "    where TKey : struct",
            "    where TValue : class",
            "{",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel<TKey, TValue> : IExample<TKey, TValue>",
            "        where TKey : struct",
            "        where TValue : class",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test,
     TestCase("class"),
     TestCase("class?"),
     TestCase("struct"),
     TestCase("notnull"),
     TestCase("System.IDisposable"),
     TestCase("System.IDisposable, System.Collections.IEnumerable"),
     TestCase("new()")]
    public void TestClassDeclarationWithGenericsAndConstraints(string constraint)
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<T>",
            $"    where T : {constraint}",
            "{",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel<T> : IExample<T>",
            $"        where T : {constraint}",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationWithGenericsAndDescriptions()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "/// <typeparam name=\"T\">Example documentation</typeparam>",
            "[Generate]",
            "interface IExample<T> { }");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    /// <typeparam name=\"T\">Example documentation</typeparam>",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel<T> : IExample<T>",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationWithMultipleGenerics()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<TKey, TValue> { }");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel<TKey, TValue> : IExample<TKey, TValue>",
            "    {",
            "    }",
            "}",
            "");
    }
}