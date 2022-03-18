using NUnit.Framework;
using static MGen.Abstractions.Generators.TestModelGenerator;

namespace MGen.Abstractions.Generators.DataBinding;

class DataBindingSupportTests
{
    [Test]
    public void TestNotifyPropertyChanged() =>
        Compile(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanged",
            "{",
            "    int Id { get; set; }",
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

    [Test]
    public void TestNotifyPropertyChangedAndChanging() =>
        Compile(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanged, INotifyPropertyChanging",
            "{",
            "    int Id { get; set; }",
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

    [Test]
    public void TestNotifyPropertyChanging() =>
        Compile(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanging",
            "{",
            "    int Id { get; set; }",
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

    [Test]
    public void TestNotifyPropertyChangingAndChanged() =>
        Compile(
            "using MGen;",
            "using System.ComponentModel;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : INotifyPropertyChanging, INotifyPropertyChanged",
            "{",
            "    int Id { get; set; }",
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