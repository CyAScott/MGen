using NUnit.Framework;
using System;

namespace MGen.Tests.InterfaceSupport
{
    [Generate]
    public interface IHaveAnId<TId>
    {
        TId Id { get; set; }
    }

    public partial class GenericInterfaceTests
    {
        [Test]
        public void TestGeneric()
        {
            var type = AssemblyScanner.FindImplementationFor(typeof(IHaveAnId<>));
            Assert.IsNotNull(type);

            var instance = Activator.CreateInstance(type.MakeGenericType(typeof(Guid))) as IHaveAnId<Guid>;
            Assert.IsNotNull(instance);
        }
    }

    [Generate]
    public interface IHaveAnGuidId : IHaveAnId<Guid>
    {
    }

    [Generate]
    public interface IHaveAnStructId<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        where T2 : struct
        where T3 : class
        where T4 : class?
        where T5 : class, new()
        where T6 : notnull, new()
        where T7 : GenericInterfaceTests
        where T8 : GenericInterfaceTests?
        where T9 : notnull, IAmInterfaceForAGenericConstraint, new()
        where T10 : GenericInterfaceTests, IAmInterfaceForAGenericConstraint?
    {
    }

    public interface IAmInterfaceForAGenericConstraint
    {
    }

    public partial class GenericInterfaceTests
    {
        [Test]
        public void TestGenericWithConstraint()
        {
            var type = AssemblyScanner.FindImplementationFor(typeof(IHaveAnStructId<,,,,,,,,,>));
            Assert.IsNotNull(type);
        }
    }
}
