using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.ReadOnlyConstructor;

class ReadOnlyConstructorSupportTests
{
    [Test]
    public void TestReadOnlyConstructorDeclaration() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample",
            "{",
            "    int Id { get; }",
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