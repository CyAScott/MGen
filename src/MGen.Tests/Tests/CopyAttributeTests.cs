using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace MGen.Tests
{
    [Generate]
    public interface IHaveCustomAttributes
    {
        [Custom(typeof(CustomAttribute), "I'm a string", CustomAttributeEnum.Test, 1, true),
         Custom(NamedArgument = new object[] { typeof(CustomAttribute), "I'm a string", CustomAttributeEnum.Test, 1, true })]
        string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class CustomAttribute : Attribute
    {
        public CustomAttribute(params object[] arguments) => NamedArgument = arguments;
        public object[] NamedArgument { get; set; }
    }

    public enum CustomAttributeEnum
    {
        Test
    }

    public class CopyAttributeTests
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveCustomAttributes>();
            Assert.IsNotNull(type);

            var properties = type.GetProperties();
            Assert.AreEqual(1, properties.Length);

            var attributes = properties[0].GetCustomAttributes<CustomAttribute>().ToArray();
            Assert.AreEqual(2, attributes.Length);

            AssertHasValues(attributes[0]);
            AssertHasValues(attributes[1]);
        }

        public void AssertHasValues(CustomAttribute attribute)
        {
            var values = attribute.NamedArgument;
            Assert.IsNotNull(values);
            Assert.AreEqual(5, values.Length);

            Assert.AreEqual(values[0], typeof(CustomAttribute));
            Assert.AreEqual(values[1], "I'm a string");
            Assert.AreEqual(values[2], CustomAttributeEnum.Test);
            Assert.AreEqual(values[3], 1);
            Assert.AreEqual(values[4], true);
        }
    }
}
