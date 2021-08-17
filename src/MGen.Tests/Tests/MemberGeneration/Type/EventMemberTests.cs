using NUnit.Framework;
using System;

namespace MGen.Tests.MemberGeneration.Type
{
    [Generate]
    public interface IHaveAnEvent
    {
        event Action Update;
    }

    public class EventMemberTests
    {
        [Test]
        public void EventTest()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveAnEvent>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IHaveAnEvent;
            Assert.IsNotNull(instance);
        }
    }
}
