using NUnit.Framework;

namespace MGen.Abstractions.Generators.ReadOnlyConstructor;

class ReadOnlyConstructorSupportTests
{
    [Test]
    public void TestReadOnlyConstructorDeclaration()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    int Id { get; }",
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
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "        }",
            "",
            "        readonly int _id;",
            "",
            "        public ExampleModel(int id)",
            "        {",
            "            _id = id;",
            "        }",
            "    }",
            "}",
            "");
    }
}