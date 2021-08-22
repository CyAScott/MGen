using NUnit.Framework;
using System;

namespace MGen.Tests.InterfaceSupport
{
    [Generate]
    public interface IAmAnInterface
    {
    }

    public class AnInterfacesTests
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IAmAnInterface>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IAmAnInterface;
            Assert.IsNotNull(instance);
        }
    }
}
