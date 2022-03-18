using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Properties;

class PropertyDeclarationTests
{
    [Test]
    public void TestIndexGetProperty() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object this[int index] { get; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object this[int index]",
            "        {",
            "            get",
            "            {",
            "                throw new System.NotImplementedException();",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestIndexSetProperty() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    object this[int index] { set; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object this[int index]",
            "        {",
            "            set",
            "            {",
            "                throw new System.NotImplementedException();",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestProperty() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    int Id { get; set; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;",
            "    }",
            "}",
            "");

    [Test]
    public void TestPropertyAttributes() =>
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
            "    int Id { get; set; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        [Example.DescriptionAttribute(\"Sample text\")]",
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;",
            "    }",
            "}",
            "");

    [Test]
    public void TestPropertyAttributesForIndexArguments() =>
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
            "    object this[[Description(\"Sample text\")] int index] { get; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public object this[[Example.DescriptionAttribute(\"Sample text\")]int index]",
            "        {",
            "            get",
            "            {",
            "                throw new System.NotImplementedException();",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestPropertyArgumentDescription() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    /// <param name=\"index\">Sample text</param>",
            "    object this[int index] { get; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        /// <param name=\"index\">Sample text</param>",
            "        public object this[int index]",
            "        {",
            "            get",
            "            {",
            "                throw new System.NotImplementedException();",
            "            }",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestPropertyDescription() =>
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
            "    int Id { get; set; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        /// <summary>",
            "        /// Sample text",
            "        /// </summary>",
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;",
            "    }",
            "}",
            "");

    [Test]
    public void TestPropertyGetAttributes() =>
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
            "    int Id { [Description(\"Sample text\")] get; set; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public int Id",
            "        {",
            "            [Example.DescriptionAttribute(\"Sample text\")]",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;",
            "    }",
            "}",
            "");

    [Test]
    public void TestPropertySetAttributes() =>
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
            "    int Id { get; [Description(\"Sample text\")] set; }",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            [Example.DescriptionAttribute(\"Sample text\")]",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;",
            "    }",
            "}",
            "");
}