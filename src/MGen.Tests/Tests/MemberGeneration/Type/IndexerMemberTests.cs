using NUnit.Framework;
using System;

namespace MGen.Tests.MemberGeneration.Type
{
    [Generate]
    public interface IHaveAnIndex
    {
        string this[int index] { get; set; }
    }

    public class IndexerMemberTests
    {
        [Test]
        public void IndexTest()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveAnIndex>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IHaveAnIndex;
            Assert.IsNotNull(instance);
        }
    }
}
