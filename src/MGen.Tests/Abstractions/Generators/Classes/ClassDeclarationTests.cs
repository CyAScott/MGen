using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Classes;

partial class ClassDeclarationTests
{
    [Test]
    public void TestClassDeclaration() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample { }")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedClass() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial class Core",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial class Core",
            "    {",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedClassInNestedClass() =>
        Compile(
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
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial class Core",
            "    {",
            "        partial class Inner",
            "        {",
            "            class ExampleModel : IExample",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedClassWithGenerics() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial class Core<T>",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial class Core<T>",
            "    {",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedInterface() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial interface ICore",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial interface ICore",
            "    {",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedInterfaceInNestedInterface() =>
        Compile(
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
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial interface ICore",
            "    {",
            "        partial interface IInner",
            "        {",
            "            class ExampleModel : IExample",
            "            {",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedInterfaceWithGenerics() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "public partial interface ICore<T>",
            "{",
            "    [Generate]",
            "    interface IExample { }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial interface ICore<T>",
            "    {",
            "        class ExampleModel : IExample",
            "        {",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestClassDeclarationInNestedNamespace() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example.Core;",
            "",
            "[Generate]",
            "interface IExample { }")
        .ShouldBe(
            "namespace Example.Core",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "    }",
            "}",
            "");
}