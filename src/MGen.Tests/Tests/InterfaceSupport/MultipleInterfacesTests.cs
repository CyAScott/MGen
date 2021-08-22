using NUnit.Framework;
using System;

namespace MGen.Tests.InterfaceSupport
{
    public interface IHaveAGetIntId
    {
        int Id { get; }
    }

    public interface IHaveAGetId
    {
        Guid Id { get; }
    }

    public interface IHaveASetId
    {
        Guid Id { set; }
    }

    [Generate]
    public interface IHaveMultipleInterfaces : IHaveAGetId, IHaveASetId, IHaveAGetIntId
    {
    }

    public class MultipleInterfacesTests
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveMultipleInterfaces>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IHaveMultipleInterfaces;
            Assert.IsNotNull(instance);
        }
    }
}
