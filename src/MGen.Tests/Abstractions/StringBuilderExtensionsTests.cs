using System.Text;
using NUnit.Framework;

namespace MGen.Abstractions;

class StringBuilderExtensionsTests
{
    [Test]
    public void TestAppendNullString()
    {
        var stringBuilder = new StringBuilder();

        const string @const = null;

        stringBuilder.AppendConstant(@const);

        Assert.AreEqual("null", stringBuilder.ToString());
    }

    [Test]
    public void TestAppendEscapedStringWithReservedCharacters()
    {
        var stringBuilder = new StringBuilder();

        const string @const = "\0\a\b\f\n\r\t\v\'\"\\";

        stringBuilder.AppendConstant(@const);

        Assert.AreEqual(@"""\0\a\b\f\n\r\t\v\'\""\\""", stringBuilder.ToString());
    }

    [Test]
    public void TestAppendEscapedString()
    {
        var stringBuilder = new StringBuilder();

        const string @const = "\u0001\u0002\u0003\u0004\u0005\u0006\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F\u007F";

        stringBuilder.AppendConstant(@const);

        Assert.AreEqual(@"""\u0001\u0002\u0003\u0004\u0005\u0006\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F\u007F""", stringBuilder.ToString());
    }

    [Test]
    public void TestAppendEscapedExtendedUnicode()
    {
        var stringBuilder = new StringBuilder();

        const string @const = "áéíóúýÁÉÍÓÚÝ";

        stringBuilder.AppendConstant(@const);

        Assert.AreEqual(@"""áéíóúýÁÉÍÓÚÝ""", stringBuilder.ToString());
    }
}