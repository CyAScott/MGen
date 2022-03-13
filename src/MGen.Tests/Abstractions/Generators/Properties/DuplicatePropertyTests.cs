using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Generators.Properties;

class DuplicatePropertyTests
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
            "interface IHaveIntProperty",
            "{",
            "    int Property { get; set; }",
            "}",
            "",
            "interface IHaveLongProperty",
            "{",
            "    long Property { get; set; }",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntProperty, IHaveLongProperty",
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
            "        public int Property",
            "        {",
            "            get",
            "            {",
            "                return _property;",
            "            }",
            "            set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        int _property;",
            "",
            "        long Example.IHaveLongProperty.Property",
            "        {",
            "            get",
            "            {",
            "                return __property;",
            "            }",
            "            set",
            "            {",
            "                __property = value;",
            "            }",
            "        }",
            "",
            "        long __property;",
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
            "interface IHaveIntProperty",
            "{",
            "    int Property { get; set; }",
            "}",
            "",
            "interface IHaveLongProperty",
            "{",
            "    long Property { get; set; }",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveLongProperty, IHaveIntProperty",
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
            "        public long Property",
            "        {",
            "            get",
            "            {",
            "                return _property;",
            "            }",
            "            set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        long _property;",
            "",
            "        int Example.IHaveIntProperty.Property",
            "        {",
            "            get",
            "            {",
            "                return __property;",
            "            }",
            "            set",
            "            {",
            "                __property = value;",
            "            }",
            "        }",
            "",
            "        int __property;",
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
            "interface IHaveIntProperty",
            "{",
            "    int Property { get; set; }",
            "}",
            "",
            "interface IHaveIntPropertyToo",
            "{",
            "    int Property { get; set; }",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntProperty, IHaveIntPropertyToo",
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
            "        public int Property",
            "        {",
            "            get",
            "            {",
            "                return _property;",
            "            }",
            "            set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        int _property;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestSameTypeAndMixedGetAndSet()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntProperty",
            "{",
            "    int Property { get; }",
            "}",
            "",
            "interface IHaveIntPropertyToo",
            "{",
            "    int Property { set; }",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntProperty, IHaveIntPropertyToo",
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
            "        public int Property",
            "        {",
            "            get",
            "            {",
            "                return _property;",
            "            }",
            "            set",
            "            {",
            "                _property = value;",
            "            }",
            "        }",
            "",
            "        int _property;",
            "    }",
            "}",
            "");
    }
}