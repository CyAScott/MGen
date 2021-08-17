using NUnit.Framework;
using System;

namespace MGen.Tests.MemberGeneration.Type
{
    [Generate]
    public interface IOnePropertyMember
    {
        Guid Id { get; set; }
    }

    public partial class PropertyMemberTests
    {
        [Test]
        public void OnePropertyTest()
        {
            var type = AssemblyScanner.FindImplementationFor<IOnePropertyMember>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IOnePropertyMember;
            Assert.IsNotNull(instance);

            var id = instance.Id = Guid.NewGuid();
            Assert.AreEqual(id, instance.Id);
        }
    }

    [Generate]
    public interface ITwoPropertyMember
    {
        Guid A { get; set; }
        Guid B { get; set; }
    }

    public partial class PropertyMemberTests
    {
        [Test]
        public void TwoPropertyTest()
        {
            var type = AssemblyScanner.FindImplementationFor<ITwoPropertyMember>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as ITwoPropertyMember;
            Assert.IsNotNull(instance);

            var a = instance.A = Guid.NewGuid();
            Assert.AreEqual(a, instance.A);

            var b = instance.B = Guid.NewGuid();
            Assert.AreEqual(b, instance.B);
        }
    }
}
