using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.Methods;

class DuplicateMethodTests
{
    [Test]
    public void TestDifferentSignatureAndDifferentReturnType() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Set(int value);",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    long Set(long value);",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveLongMethod",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public int Set(int value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        public long Set(long value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestDifferentSignatureAndSameReturnType() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    void Set(int value);",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    void Set(long value);",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveLongMethod",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public void Set(int value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        public void Set(long value)",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestSameSignatureAndDifferentReturnType() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Get();",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    long Get();",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveLongMethod",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public int Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        long Example.IHaveLongMethod.Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestSameSignatureAndDifferentReturnTypeAndOrderOfInterfaces() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Get();",
            "}",
            "",
            "interface IHaveLongMethod",
            "{",
            "    long Get();",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveLongMethod, IHaveIntMethod",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public long Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "",
            "        int Example.IHaveIntMethod.Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");

    [Test]
    public void TestSameSignatureAndSameReturnType() =>
        Compile(
            "using MGen;",
            "using System;",
            "",
            "namespace Example;",
            "",
            "interface IHaveIntMethod",
            "{",
            "    int Get();",
            "}",
            "",
            "interface IHaveIntMethodToo",
            "{",
            "    int Get();",
            "}",
            "",
            "[Generate]",
            "interface IExample : IHaveIntMethod, IHaveIntMethodToo",
            "{",
            "}")
        .ShouldBe(
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {",
            "        public int Get()",
            "        {",
            "            throw new System.NotImplementedException();",
            "        }",
            "    }",
            "}",
            "");
}