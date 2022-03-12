using MGen.Abstractions;
using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace MGen;

[DebuggerStepThrough]
static class TestExtensions
{
    public static string ToCode(this IAmCode code)
    {
        var builder = new StringBuilder();

        builder.AppendCode(code);

        return builder.ToString();
    }

    public static void ShouldBe(this string code, params string[] lines)
    {
        var expected = string.Join(Environment.NewLine, lines);

        TestContext.Out.WriteLine("Actual:");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine(code);

        TestContext.Out.WriteLine("Expected:");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine(expected);

        Assert.AreEqual(expected, code);
    }
}