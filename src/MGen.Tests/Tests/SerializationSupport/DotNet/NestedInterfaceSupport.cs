using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace MGen.Tests.SerializationSupport.DotNet
{
    public interface INestedSerializableChild : ISerializable
    {
        Guid Id { get; set; }
    }

    [Generate]
    public interface INestedSerializableTest : ISerializable
    {
        Guid Id { get; set; }
        INestedSerializableChild Child { get; set; }
    }

    public class NestedInterfaceSupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<INestedSerializableTest>();
            Assert.IsNotNull(type);

            var instance = (INestedSerializableTest)Activator.CreateInstance(type);

            var id = instance.Id = Guid.NewGuid();

            var childType = AssemblyScanner.FindImplementationFor<INestedSerializableChild>();
            Assert.IsNotNull(childType);
            var child = instance.Child = (INestedSerializableChild)Activator.CreateInstance(childType);

            var childId = child.Id = Guid.NewGuid();

            var clone = instance.Clone();

            Assert.IsNotNull(clone);
            Assert.IsFalse(ReferenceEquals(instance.Child, clone.Child));

            Assert.AreEqual(id, clone.Id);
            Assert.AreEqual(childId, clone.Child?.Id);
        }
    }
}
