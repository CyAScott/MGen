using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Methods;

class MethodDeclarationTests
{
    [Test]
    public void TestMethod() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object Get();",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodAttributes() =>
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
            "    object Get();",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        [Example.DescriptionAttribute(\"Sample text\")]",
            "        public object Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodAttributesForArguments() =>
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
            "    object Get([Description(\"Sample text\")] object arg);",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get([Example.DescriptionAttribute(\"Sample text\")]object arg)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodAttributesForGenericArguments() =>
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
            "    object Get<[Description(\"Sample text\")] T>();",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object Get<[Example.DescriptionAttribute(\"Sample text\")]T>()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodArgumentDescription() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    /// <param name=\"arg\">Sample text</param>",
            "    object Get(object arg);",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        /// <param name=\"arg\">Sample text</param>",
            "        public object Get(object arg)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodDescription() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    /// <summary>",
            "    /// Sample text",
            "    /// </summary>",
            "    object Get();",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        /// <summary>",
            "        /// Sample text",
            "        /// </summary>",
            "        public object Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodForPartialClass() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "partial interface IExample",
            "{",
            "    object Get();",
            "}",
            "",
            "partial class ExampleModel",
            "{",
            "    public partial object Get() => null!;",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    partial class ExampleModel : IExample",
            "    {",
            "        public partial object Get();",
            "    }",
            "}",
            "");

    [Test]
    public void TestMethodGenericArgumentDescription() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    /// <typeparam name=\"T\">Sample text</typeparam>",
            "    object Get<T>();",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        /// <typeparam name=\"T\">Sample text</typeparam>",
            "        public object Get<T>()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
}