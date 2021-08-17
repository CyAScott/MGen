using NUnit.Framework;
using System;

namespace MGen.Tests.MemberGeneration.Type
{
    [Generate]
    public interface IHaveAMethod
    {
        void Test();
    }

    public class MethodMemberTests
    {
        [Test]
        public void MethodTest()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveAMethod>();
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type) as IHaveAMethod;
            Assert.IsNotNull(instance);
            Assert.Throws<NotImplementedException>(instance.Test);
        }
    }
}
