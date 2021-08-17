using NUnit.Framework;
using System;

namespace MGen.Tests.CloningSupport
{
    public interface INestedCloneChild : ICloneable
    {
        Guid Id { get; set; }
    }

    [Generate]
    public interface INestedCloneTest : ICloneable
    {
        Guid Id { get; set; }
        INestedCloneChild Child { get; set; }
    }

    public class NestedInterfaceSupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<INestedCloneTest>();
            Assert.IsNotNull(type);

            var instance = (INestedCloneTest)Activator.CreateInstance(type);

            var id = instance.Id = Guid.NewGuid();

            var childType = AssemblyScanner.FindImplementationFor<INestedCloneChild>();
            Assert.IsNotNull(childType);
            var child = instance.Child = (INestedCloneChild)Activator.CreateInstance(childType);

            var childId = child.Id = Guid.NewGuid();

            var clone = (INestedCloneTest)instance.Clone();

            Assert.IsNotNull(clone);
            Assert.IsFalse(ReferenceEquals(instance.Child, clone.Child));

            Assert.AreEqual(id, clone.Id);
            Assert.AreEqual(childId, clone.Child?.Id);
        }
    }
}
