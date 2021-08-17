using NUnit.Framework;
using System;
using System.ComponentModel;

namespace MGen.Tests.DataBindingSupport
{
    [Generate]
    public interface ISupportNotifyPropertyChanged : INotifyPropertyChanged
    {
        Guid Id { get; set; }
    }

    public class NotifyPropertyChangedSupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<ISupportNotifyPropertyChanged>();
            Assert.IsNotNull(type);

            var instance = (ISupportNotifyPropertyChanged)Activator.CreateInstance(type);

            var id = Guid.NewGuid();

            var eventInvoked = false;
            instance.PropertyChanged += (sender, e) =>
            {
                eventInvoked = true;
                Assert.AreEqual(id, instance.Id);
            };

            instance.Id = id;

            Assert.IsTrue(eventInvoked);
        }
    }
}
