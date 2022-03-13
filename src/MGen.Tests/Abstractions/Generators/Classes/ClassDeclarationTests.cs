using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test]
    public void TestClassDeclaration()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample { }");

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
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedClass()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial class Core",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    partial class Core",
            "    {",
            "        [MGen.GenerateAttribute]",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedClassInNestedClass()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial class Core",
            "{",
            "    public partial class Inner",
            "    {",
            "        [Generate]",
            "        interface IExample { }",
            "    }",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    partial class Core",
            "    {",
            "        partial class Inner",
            "        {",
            "            [MGen.GenerateAttribute]",
            "            class ExampleModel : IExample",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedClassWithGenerics()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial class Core<T>",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    partial class Core<T>",
            "    {",
            "        [MGen.GenerateAttribute]",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedInterface()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial interface ICore",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    partial interface ICore",
            "    {",
            "        [MGen.GenerateAttribute]",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedInterfaceInNestedInterface()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial interface ICore",
            "{",
            "    public partial interface IInner",
            "    {",
            "        [Generate]",
            "        interface IExample { }",
            "    }",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    partial interface ICore",
            "    {",
            "        partial interface IInner",
            "        {",
            "            [MGen.GenerateAttribute]",
            "            class ExampleModel : IExample",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedInterfaceWithGenerics()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial interface ICore<T>",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example",
            "{",
            "    partial interface ICore<T>",
            "    {",
            "        [MGen.GenerateAttribute]",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestClassDeclarationInNestedNamespace()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example.Core;",
            "",
            "[Generate]",
            "interface IExample { }");

        string? contents = null;
        testModelGenerator.FileGenerated += args => contents = args.Contents;

        testModelGenerator.Compile(out var diagnostics);
        diagnostics.ShouldBeEmpty();

        contents.ShouldBe(
            "namespace Example.Core",
            "{",
            "    [MGen.GenerateAttribute]",
            "    class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");
    }
}