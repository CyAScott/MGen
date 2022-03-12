using System.Text;

namespace MGen.Abstractions;

class TestParent : IAmIndentedCode
{
    public TestParent(int indentLevel = 1) => IndentLevel = indentLevel;

    void IAmCode.Generate(StringBuilder stringBuilder)
    {
    }

    public int IndentLevel { get; }
}