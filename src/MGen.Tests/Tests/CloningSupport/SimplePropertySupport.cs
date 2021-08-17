using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MGen.Tests.CloningSupport
{
    [Generate]
    public interface IHaveASimplePorperty : ICloneable
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
            var type = AssemblyScanner.FindImplementationFor<IHaveASimplePorperty>();
            Assert.IsNotNull(type);

            var constructers = type.GetConstructors();
            Assert.AreEqual(2, constructers.Length);

            var defaultCtor = constructers.SingleOrDefault(it => it.GetParameters().Length == 0);
            Assert.IsNotNull(defaultCtor);

            var cloneCtor = constructers.SingleOrDefault(it => it.GetParameters().Length == 1);
            Assert.IsNotNull(cloneCtor);

            var parameters = cloneCtor.GetParameters();
            Assert.AreEqual(type, parameters[0].ParameterType);

            Assert.IsTrue(Attribute.IsDefined(parameters[0], typeof(NotNullAttribute)));

            var instanceA = defaultCtor.Invoke(new object[0]) as IHaveASimplePorperty;
            Assert.IsNotNull(instanceA);

            var dateTime = instanceA.DateTime = DateTime.UtcNow;
            var id = instanceA.Id = Guid.NewGuid();
            var simpleEnum = instanceA.SimpleEnum = SimpleEnum.One;
            var integer = instanceA.Integer = 3;
            var @string = instanceA.String = "Hello World";

            var instanceB = instanceA.Clone() as IHaveASimplePorperty;
            Assert.IsNotNull(instanceB);
            Assert.IsFalse(ReferenceEquals(instanceA, instanceB));
            Assert.AreEqual(dateTime, instanceB.DateTime);
            Assert.AreEqual(id, instanceB.Id);
            Assert.AreEqual(simpleEnum, instanceB.SimpleEnum);
            Assert.AreEqual(integer, instanceB.Integer);
            Assert.AreEqual(@string, instanceB.String);

            var instanceC = cloneCtor.Invoke(new object[] { instanceA }) as IHaveASimplePorperty;
            Assert.IsNotNull(instanceC);
            Assert.AreEqual(dateTime, instanceC.DateTime);
            Assert.AreEqual(id, instanceC.Id);
            Assert.AreEqual(simpleEnum, instanceC.SimpleEnum);
            Assert.AreEqual(integer, instanceC.Integer);
            Assert.AreEqual(@string, instanceC.String);
        }
    }
}
