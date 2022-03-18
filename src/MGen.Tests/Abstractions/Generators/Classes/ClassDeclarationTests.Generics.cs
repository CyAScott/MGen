using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test]
    public void TestClassDeclarationWithGenerics() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<T> { }")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel<T> : IExample<T>",
            "    {",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationWithGenericsAndConstraints() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<TKey, TValue>",
            "    where TKey : struct",
            "    where TValue : class",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel<TKey, TValue> : IExample<TKey, TValue>",
            "        where TKey : struct",
            "        where TValue : class",
            "    {",
            "    }",
            "}",
            "");

    [Test,
     TestCase("class"),
     TestCase("class?"),
     TestCase("struct"),
     TestCase("notnull"),
     TestCase("System.IDisposable"),
     TestCase("System.IDisposable, System.Collections.IEnumerable"),
     TestCase("new()")]
    public void TestClassDeclarationWithGenericsAndConstraints(string constraint) =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<T>",
            $"    where T : {constraint}",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel<T> : IExample<T>",
            $"        where T : {constraint}",
            "    {",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationWithGenericsAndDescriptions() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "/// <typeparam name=\"T\">Example documentation</typeparam>",
            "[Generate]",
            "interface IExample<T> { }")
        .ShouldBe(
            "namespace Example",
            "{",
            "    /// <typeparam name=\"T\">Example documentation</typeparam>",
            "    class ExampleModel<T> : IExample<T>",
            "    {",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationWithMultipleGenerics() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample<TKey, TValue> { }")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel<TKey, TValue> : IExample<TKey, TValue>",
            "    {",
            "    }",
            "}",
            "");
}