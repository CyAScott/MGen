using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using static MGen.Abstractions.Generators.Conversion.ConversionSupportTestResults;

namespace MGen.Abstractions.Generators.Conversion;

[DebuggerStepThrough]
class ConversionSupportTestResults
{
    public static ConversionSupportTestResults Compile(params string[] lines) =>
        new()
        {
            _contents = TestModelGenerator.Compile(lines)
        };

    string _contents = string.Empty;

    string[] _ctor = Array.Empty<string>();
    public ConversionSupportTestResults Ctor(params string[] lines)
    {
        _ctor = lines;
        return this;
    }

    string[] _properties = Array.Empty<string>();
    public ConversionSupportTestResults Properties(params string[] lines)
    {
        _properties = lines;
        return this;
    }

    string[] _tryGetValue = Array.Empty<string>();
    public ConversionSupportTestResults TryGetValue(params string[] lines)
    {
        _tryGetValue = lines;
        return this;
    }

    public void ValidateCode()
    {
        var lines = new List<string>
        {
            "namespace Example",
            "{",
            "    class ExampleModel : IExample",
            "    {"
        };

        lines.AddRange(_properties);

        lines.Add("");

        lines.AddRange(_tryGetValue);

        lines.AddRange(new[]
        {
            "",
            "        System.TypeCode System.IConvertible.GetTypeCode()",
            "        {",
            "            return System.TypeCode.Object;",
            "        }",
            "",
            "        bool System.IConvertible.ToBoolean(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        char System.IConvertible.ToChar(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        sbyte System.IConvertible.ToSByte(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        byte System.IConvertible.ToByte(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        short System.IConvertible.ToInt16(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        ushort System.IConvertible.ToUInt16(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        int System.IConvertible.ToInt32(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        uint System.IConvertible.ToUInt32(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        long System.IConvertible.ToInt64(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        ulong System.IConvertible.ToUInt64(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        float System.IConvertible.ToSingle(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        double System.IConvertible.ToDouble(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        decimal System.IConvertible.ToDecimal(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider? provider)",
            "        {",
            "            throw new System.NotSupportedException();",
            "        }",
            "",
            "        string System.IConvertible.ToString(System.IFormatProvider? provider)",
            "        {",
            "            return ToString()!;",
            "        }",
            "",
            "        object System.IConvertible.ToType(System.Type conversionType, System.IFormatProvider? provider)",
            "        {",
            "            foreach (var constructor in conversionType.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))",
            "            {",
            "                var parameters = constructor.GetParameters();",
            "                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(MGen.ISupportConversion)))",
            "                {",
            "                    return constructor.Invoke(new object[] { this });",
            "                }",
            "            }",
            "            throw new System.InvalidCastException();",
            "        }",
            ""
        });

        lines.AddRange(_ctor);

        lines.AddRange(new []
        {
            "    }",
            "}",
            ""
        });

        _contents.ShouldBe(lines.ToArray());
    }
}

class ConversionSupportTests
{
    //todo: when nested class generation is supported, test nested class convert type

    [Test]
    public void TestConvertRefType() =>
        Compile(
                "using MGen;",
                "using System;",
                "",
                "namespace Example;",
                "",
                "[Generate]",
                "interface IExample : ISupportConversion",
                "{",
                "    IDisposable Resource { get; set; }",
                "}")
            .Properties(
                "        public System.IDisposable Resource",
                "        {",
                "            get",
                "            {",
                "                return _resource;",
                "            }",
                "            set",
                "            {",
                "                _resource = value;",
                "            }",
                "        }",
                "",
                "        System.IDisposable _resource;")
            .TryGetValue(
                "        bool MGen.ISupportConversion.TryGetValue(string name, out object? value)",
                "        {",
                "            switch (name)",
                "            {",
                "                default:",
                "                    {",
                "                        value = null;",
                "                        return false;",
                "                    }",
                "                case \"Resource\":",
                "                    {",
                "                        value = _resource;",
                "                        return true;",
                "                    }",
                "            }",
                "        }")
            .Ctor(
                "        protected ExampleModel([System.Diagnostics.CodeAnalysis.NotNullAttribute]MGen.ISupportConversion obj)",
                "        {",
                "            if (obj == null) throw new System.ArgumentNullException(\"obj\");",
                "",
                "            object? value;",
                "",
                "            _resource = !obj.TryGetValue(\"Resource\", out value) || value == null ? default : (System.IDisposable)System.Convert.ChangeType(value, typeof(System.IDisposable));",
                "        }")
            .ValidateCode();

    [Test]
    public void TestConvertStringType() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : ISupportConversion",
            "{",
            "    string Id { get; set; }",
            "}")
        .Properties(
            "        public string Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        string _id;")
        .TryGetValue(
            "        bool MGen.ISupportConversion.TryGetValue(string name, out object? value)",
            "        {",
            "            switch (name)",
            "            {",
            "                default:",
            "                    {",
            "                        value = null;",
            "                        return false;",
            "                    }",
            "                case \"Id\":",
            "                    {",
            "                        value = _id;",
            "                        return true;",
            "                    }",
            "            }",
            "        }")
        .Ctor(
            "        protected ExampleModel([System.Diagnostics.CodeAnalysis.NotNullAttribute]MGen.ISupportConversion obj)",
            "        {",
            "            if (obj == null) throw new System.ArgumentNullException(\"obj\");",
            "",
            "            object? value;",
            "",
            "            _id = !obj.TryGetValue(\"Id\", out value) ? default : value as string ?? value?.ToString();",
            "        }")
        .ValidateCode();

    [Test]
    public void TestConvertValueType() =>
        Compile(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample : ISupportConversion",
            "{",
            "    int Id { get; set; }",
            "}")
        .Properties(
            "        public int Id",
            "        {",
            "            get",
            "            {",
            "                return _id;",
            "            }",
            "            set",
            "            {",
            "                _id = value;",
            "            }",
            "        }",
            "",
            "        int _id;")
        .TryGetValue(
            "        bool MGen.ISupportConversion.TryGetValue(string name, out object? value)",
            "        {",
            "            switch (name)",
            "            {",
            "                default:",
            "                    {",
            "                        value = null;",
            "                        return false;",
            "                    }",
            "                case \"Id\":",
            "                    {",
            "                        value = _id;",
            "                        return true;",
            "                    }",
            "            }",
            "        }")
        .Ctor(
            "        protected ExampleModel([System.Diagnostics.CodeAnalysis.NotNullAttribute]MGen.ISupportConversion obj)",
            "        {",
            "            if (obj == null) throw new System.ArgumentNullException(\"obj\");",
            "",
            "            object? value;",
            "",
            "            _id = !obj.TryGetValue(\"Id\", out value) ? default : value as int? ?? (int)System.ComponentModel.TypeDescriptor.GetConverter(typeof(int)).ConvertFrom(value);",
            "        }")
        .ValidateCode();
}