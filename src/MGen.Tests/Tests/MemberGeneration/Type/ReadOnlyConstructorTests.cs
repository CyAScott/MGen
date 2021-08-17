using NUnit.Framework;
using System;

namespace MGen.Tests.MemberGeneration.Type
{
    [Generate]
    public interface IHaveAReadOnlyProperty
    {
        Guid Id { get; }
    }

    public class ReadOnlyConstructorTests
    {
        [Test]
        public void ReadOnlyPropertyTest()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveAReadOnlyProperty>();
            Assert.IsNotNull(type);

            var constructors = type.GetConstructors();
            Assert.AreEqual(1, constructors.Length);

            var parameters = constructors[0].GetParameters();
            Assert.AreEqual(1, parameters.Length);

            Assert.AreEqual(typeof(Guid), parameters[0].ParameterType);

            var id = Guid.NewGuid();

            var instance = constructors[0].Invoke(new object[] { id }) as IHaveAReadOnlyProperty;
            Assert.IsNotNull(instance);

            Assert.AreEqual(id, instance.Id);
        }
    }
}
