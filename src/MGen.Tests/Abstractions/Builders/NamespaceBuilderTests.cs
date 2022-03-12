using NUnit.Framework;

namespace MGen.Abstractions.Builders;

class NamespaceBuilderTests
{
    [Test]
    public void CreateNamespace()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "}",
            "");
    }

    [Test]
    public void CreateNamespaceWithUsingStatements()
    {
        var @namespace = new NamespaceBuilder("Test");
        @namespace.Usings.Add("System");
        @namespace.Usings.Add("log = System.Console");
        @namespace.Usings.Add("static System.Convert");

        @namespace.ToCode().ShouldBe(
            "using System;",
            "using log = System.Console;",
            "using static System.Convert;",
            "",
            "namespace Test",
            "{",
            "}",
            "");
    }

    [Test]
    public void CreateNestedNamespace()
    {
        var @namespace = new NamespaceBuilder("Test");

        @namespace.AddNamespace("Nested");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    namespace Nested",
            "    {",
            "    }",
            "}",
            "");
    }

    [Test]
    public void CreateNestedNamespaceWithUsingStatements()
    {
        var @namespace = new NamespaceBuilder("Test");

        var nestedNamespace = @namespace.AddNamespace("Nested");
        nestedNamespace.Usings.Add("System");
        nestedNamespace.Usings.Add("log = System.Console");
        nestedNamespace.Usings.Add("static System.Convert");

        @namespace.ToCode().ShouldBe(
            "namespace Test",
            "{",
            "    using System;",
            "    using log = System.Console;",
            "    using static System.Convert;",
            "    ",
            "    namespace Nested",
            "    {",
            "    }",
            "}",
            "");
    }
}