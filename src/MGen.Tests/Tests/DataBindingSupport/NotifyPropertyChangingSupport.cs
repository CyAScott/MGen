using NUnit.Framework;
using System;
using System.ComponentModel;

namespace MGen.Tests.DataBindingSupport
{
    [Generate]
    public interface ISupportNotifyPropertyChanging : INotifyPropertyChanging
    {
        Guid Id { get; set; }
    }

    public class NotifyPropertyChangingSupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<ISupportNotifyPropertyChanging>();
            Assert.IsNotNull(type);

            var instance = (ISupportNotifyPropertyChanging)Activator.CreateInstance(type);

            var id = Guid.NewGuid();

            var eventInvoked = false;
            instance.PropertyChanging += (sender, e) =>
            {
                eventInvoked = true;
                Assert.AreEqual(Guid.Empty, instance.Id);
            };

            instance.Id = id;

            Assert.AreEqual(id, instance.Id);
            Assert.IsTrue(eventInvoked);
        }
    }
}
