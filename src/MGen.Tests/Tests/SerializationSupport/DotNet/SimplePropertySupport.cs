using NUnit.Framework;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace MGen.Tests.SerializationSupport.DotNet
{
    [Generate]
    public interface IHaveASimpleProperty : ISerializable
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

    public class SimplePropertySupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveASimpleProperty>();
            Assert.IsNotNull(type);

            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(defaultCtor);

            var serializationCtor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null,
                new[] { typeof(SerializationInfo), typeof(StreamingContext) },
                null);
            Assert.IsNotNull(serializationCtor);

            var instanceA = defaultCtor.Invoke(new object[0]) as IHaveASimpleProperty;
            Assert.IsNotNull(instanceA);

            var dateTime = instanceA.DateTime = DateTime.UtcNow;
            var id = instanceA.Id = Guid.NewGuid();
            var simpleEnum = instanceA.SimpleEnum = SimpleEnum.One;
            var integer = instanceA.Integer = 3;
            var @string = instanceA.String = "Hello World";

            var instanceB = instanceA.CloneViaDotNetSerialization();
            Assert.IsNotNull(instanceB);
            Assert.IsFalse(ReferenceEquals(instanceA, instanceB));
            Assert.AreEqual(dateTime, instanceB.DateTime);
            Assert.AreEqual(id, instanceB.Id);
            Assert.AreEqual(simpleEnum, instanceB.SimpleEnum);
            Assert.AreEqual(integer, instanceB.Integer);
            Assert.AreEqual(@string, instanceB.String);
        }
    }
}
