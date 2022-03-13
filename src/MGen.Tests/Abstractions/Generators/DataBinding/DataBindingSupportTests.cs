using NUnit.Framework;
using Shouldly;

namespace MGen.Abstractions.Generators.DataBinding;

class DataBindingSupportTests
{
    [Test]
    public void TestNotifyPropertyChanged()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanged",
            "{",
            "    int Id { get; set; }",
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
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(\"Id\"));",
            "            }",
            "        }",
            "",
            "        int _id;",
            "",
            "        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestNotifyPropertyChangedAndChanging()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanged, INotifyPropertyChanging",
            "{",
            "    int Id { get; set; }",
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
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"Id\"));",
            "                _id = value;",
            "                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(\"Id\"));",
            "            }",
            "        }",
            "",
            "        int _id;",
            "",
            "        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;",
            "",
            "        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestNotifyPropertyChanging()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanging",
            "{",
            "    int Id { get; set; }",
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
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"Id\"));",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;",
            "",
            "        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;",
            "    }",
            "}",
            "");
    }

    [Test]
    public void TestNotifyPropertyChangingAndChanged()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanging, INotifyPropertyChanged",
            "{",
            "    int Id { get; set; }",
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
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"Id\"));",
            "                _id = value;",
            "                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(\"Id\"));",
            "            }",
            "        }",
            "",
            "        int _id;",
            "",
            "        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;",
            "",
            "        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;",
            "    }",
            "}",
            "");
    }
}