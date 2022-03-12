using NUnit.Framework;

namespace MGen.Abstractions.Builders.Components;

class AttributesTests
{
    [Test, TestCase(false), TestCase(true)]
    public void TestMultipleAttributes(bool appendNewLineBetweenEachAttribute)
    {
        var attributes = new Attributes(new TestParent(), appendNewLineBetweenEachAttribute);

        var attribute1 = attributes.Add("Example1");
        attribute1.Arguments.Add(new Code(sb => sb.AppendConstant("Hello World")));

        var attribute2 = attributes.Add("Example2");
        attribute2.Arguments.Add(new Code(sb => sb.AppendConstant("Hello World")));
        attribute2.Arguments.Add(10);
        attribute2.NamedParameters.Add("Value1", new Code(sb => sb.AppendConstant("Hello Worlds")));
        attribute2.NamedParameters.Add("Value2", 11);

        var attribute3 = attributes.Add("Example3");
        attribute3.Arguments.Add(new Code(sb => sb.AppendConstant("Hello World")));
        attribute3.Arguments.Add(10);

        var attribute4 = attributes.Add("Example4");
        attribute4.NamedParameters.Add("Value1", new Code(sb => sb.AppendConstant("Hello World")));

        var attribute5 = attributes.Add("Example5");
        attribute5.NamedParameters.Add("Value1", new Code(sb => sb.AppendConstant("Hello World")));
        attribute5.NamedParameters.Add("Value2", 10);
        
        attributes.Add("Example6");

        var code = attributes.ToCode();

        if (appendNewLineBetweenEachAttribute)
        {
            code.ShouldBe(
                "    [Example1(\"Hello World\")]",
                "    [Example2(\"Hello World\", 10, Value1 = \"Hello Worlds\", Value2 = 11)]",
                "    [Example3(\"Hello World\", 10)]",
                "    [Example4(Value1 = \"Hello World\")]",
                "    [Example5(Value1 = \"Hello World\", Value2 = 10)]",
                "    [Example6]",
                "");
        }
        else
        {
            code.ShouldBe("[Example1(\"Hello World\")][Example2(\"Hello World\", 10, Value1 = \"Hello Worlds\", Value2 = 11)][Example3(\"Hello World\", 10)][Example4(Value1 = \"Hello World\")][Example5(Value1 = \"Hello World\", Value2 = 10)][Example6]");
        }
    }

    [Test]
    public void TestArgument()
    {
        var attributes = new Attributes(new TestParent(), false);

        var attribute = attributes.Add("Example");
        attribute.Arguments.Add(new Code(sb => sb.AppendConstant("Hello World")));

        var code = attributes.ToCode();

        code.ShouldBe("[Example(\"Hello World\")]");
    }

    [Test]
    public void TestArgumentsAndNamedParameters()
    {
        var attributes = new Attributes(new TestParent(), false);

        var attribute = attributes.Add("Example");
        attribute.Arguments.Add(new Code(sb => sb.AppendConstant("Hello World")));
        attribute.Arguments.Add(10);
        attribute.NamedParameters.Add("Value1", new Code(sb => sb.AppendConstant("Hello Worlds")));
        attribute.NamedParameters.Add("Value2", 11);

        var code = attributes.ToCode();

        code.ShouldBe("[Example(\"Hello World\", 10, Value1 = \"Hello Worlds\", Value2 = 11)]");
    }

    [Test]
    public void TestArguments()
    {
        var attributes = new Attributes(new TestParent(), false);

        var attribute = attributes.Add("Example");
        attribute.Arguments.Add(new Code(sb => sb.AppendConstant("Hello World")));
        attribute.Arguments.Add(10);

        var code = attributes.ToCode();

        code.ShouldBe("[Example(\"Hello World\", 10)]");
    }

    [Test]
    public void TestDisabled()
    {
        var attributes = new Attributes(new TestParent(), false);

        attributes.Add("Example1").Enabled = false;
        attributes.Add("Example2");

        var code = attributes.ToCode();

        code.ShouldBe("[Example2]");
    }

    [Test]
    public void TestNamedParameter()
    {
        var attributes = new Attributes(new TestParent(), false);

        var attribute = attributes.Add("Example");
        attribute.NamedParameters.Add("Value1", new Code(sb => sb.AppendConstant("Hello World")));

        var code = attributes.ToCode();

        code.ShouldBe("[Example(Value1 = \"Hello World\")]");
    }

    [Test]
    public void TestNamedParameters()
    {
        var attributes = new Attributes(new TestParent(), false);

        var attribute = attributes.Add("Example");
        attribute.NamedParameters.Add("Value1", new Code(sb => sb.AppendConstant("Hello World")));
        attribute.NamedParameters.Add("Value2", 10);

        var code = attributes.ToCode();

        code.ShouldBe("[Example(Value1 = \"Hello World\", Value2 = 10)]");
    }

    [Test]
    public void TestNoArgumentsOrNamedParameters()
    {
        var attributes = new Attributes(new TestParent(), false)
        {
            "Example"
        };

        var code = attributes.ToCode();

        code.ShouldBe("[Example]");
    }
}