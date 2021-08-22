using NUnit.Framework;
using System;

namespace MGen.Tests.InterfaceSupport
{
    public interface IHaveAnIdAndName
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }

    [Generate]
    public interface IHaveUsers
    {
        IHaveAnIdAndName[] Users { get; set; }
    }

    public class NestedInterfacesTests
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveUsers>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IHaveUsers;
            Assert.IsNotNull(instance);

            var nestedType = AssemblyScanner.FindImplementationFor<IHaveAnIdAndName>();
            Assert.IsNotNull(nestedType);
        }
    }
}
