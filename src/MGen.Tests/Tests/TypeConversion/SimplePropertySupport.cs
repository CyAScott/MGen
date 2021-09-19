using NUnit.Framework;
using System;

namespace MGen.Tests.TypeConversion
{
    [Generate]
    public interface IHaveASimpleProperty : ISupportConversion
    {
        DateTime DateTime { get; set; }
        Guid Id { get; set; }
        SimpleEnum SimpleEnum { get; set; }
        int Integer { get; set; }
        string String { get; set; }
    }

    public enum SimpleEnum
    {
        Zero = 0,
        One = 1
    }

    [Generate]
    public interface IHaveCommonPropertiesToSimpleProperty : ISupportConversion
    {
        DateTime DateTime { get; set; }
        Guid Id { get; set; }
        SimpleEnum SimpleEnum { get; set; }
        int Integer { get; set; }
        string String { get; set; }
    }

    [Generate]
    public interface IHaveCommonPropertiesAsStringsToSimpleProperty : ISupportConversion
    {
        string DateTime { get; set; }
        string Id { get; set; }
        string SimpleEnum { get; set; }
        string Integer { get; set; }
        string String { get; set; }
    }

    public class SimplePropertySupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveASimpleProperty>();
            Assert.IsNotNull(type);

            var original = Activator.CreateInstance(type) as IHaveASimpleProperty;
            Assert.IsNotNull(original);

            var dateTime = original.DateTime = DateTime.UtcNow;
            var id = original.Id = Guid.NewGuid();
            var simpleEnum = original.SimpleEnum = SimpleEnum.One;
            var integer = original.Integer = 3;
            var @string = original.String = "Hello World";

            var commonPropertiesType = AssemblyScanner.FindImplementationFor<IHaveCommonPropertiesToSimpleProperty>();
            Assert.IsNotNull(commonPropertiesType);
            var commonPropertiesInstance = Convert.ChangeType(original, commonPropertiesType) as IHaveCommonPropertiesToSimpleProperty;
            Assert.IsNotNull(commonPropertiesInstance);
            Assert.AreEqual(dateTime, commonPropertiesInstance.DateTime);
            Assert.AreEqual(id, commonPropertiesInstance.Id);
            Assert.AreEqual(simpleEnum, commonPropertiesInstance.SimpleEnum);
            Assert.AreEqual(integer, commonPropertiesInstance.Integer);
            Assert.AreEqual(@string, commonPropertiesInstance.String);

            var commonPropertiesAsStringsType = AssemblyScanner.FindImplementationFor<IHaveCommonPropertiesAsStringsToSimpleProperty>();
            Assert.IsNotNull(commonPropertiesAsStringsType);
            var commonPropertiesAsStringsInstance = Convert.ChangeType(original, commonPropertiesAsStringsType) as IHaveCommonPropertiesAsStringsToSimpleProperty;
            Assert.IsNotNull(commonPropertiesAsStringsInstance);
            Assert.AreEqual(dateTime.ToString(), commonPropertiesAsStringsInstance.DateTime);
            Assert.AreEqual(id.ToString(), commonPropertiesAsStringsInstance.Id);
            Assert.AreEqual(simpleEnum.ToString(), commonPropertiesAsStringsInstance.SimpleEnum);
            Assert.AreEqual(integer.ToString(), commonPropertiesAsStringsInstance.Integer);
            Assert.AreEqual(@string.ToString(), commonPropertiesAsStringsInstance.String);

            var originalFromCopy = Convert.ChangeType(commonPropertiesAsStringsInstance, type) as IHaveASimpleProperty;
            Assert.IsNotNull(originalFromCopy);
            Assert.AreEqual(dateTime.ToString(), originalFromCopy.DateTime.ToString());
            Assert.AreEqual(id, originalFromCopy.Id);
            Assert.AreEqual(simpleEnum, originalFromCopy.SimpleEnum);
            Assert.AreEqual(integer, originalFromCopy.Integer);
            Assert.AreEqual(@string, originalFromCopy.String);
        }
    }
}
