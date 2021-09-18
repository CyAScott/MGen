using NUnit.Framework;
using System;

namespace MGen.Tests.TypeConversion
{
    public interface INestedConversionChild : ISupportConversion
    {
        Guid Id { get; set; }
    }

    [Generate]
    public interface INestedConversionTest : ISupportConversion
    {
        Guid Id { get; set; }
        INestedConversionChild Child { get; set; }
    }

    public interface INestedConversionChildAsStrings : ISupportConversion
    {
        string Id { get; set; }
    }

    [Generate]
    public interface INestedConversionTestAsStrings : ISupportConversion
    {
        string Id { get; set; }
        INestedConversionChildAsStrings Child { get; set; }
    }

    public class NestedInterfaceSupport
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<INestedConversionTest>();
            Assert.IsNotNull(type);

            var original = (INestedConversionTest)Activator.CreateInstance(type);

            var id = original.Id = Guid.NewGuid();

            var childType = AssemblyScanner.FindImplementationFor<INestedConversionChild>();
            Assert.IsNotNull(childType);
            var child = original.Child = (INestedConversionChild)Activator.CreateInstance(childType);

            var childId = child.Id = Guid.NewGuid();

            var typeAsStrings = AssemblyScanner.FindImplementationFor<INestedConversionTestAsStrings>();
            Assert.IsNotNull(typeAsStrings);

            var typeAsStringsInstance = Convert.ChangeType(original, typeAsStrings) as INestedConversionTestAsStrings;
            Assert.IsNotNull(typeAsStringsInstance);

            Assert.AreEqual(id.ToString(), typeAsStringsInstance.Id);
            Assert.AreEqual(childId.ToString(), typeAsStringsInstance.Child?.Id);

            var originalFromCopy = Convert.ChangeType(typeAsStringsInstance, type) as INestedConversionTest;
            Assert.IsNotNull(originalFromCopy);

            Assert.AreEqual(id, originalFromCopy.Id);
            Assert.AreEqual(childId, originalFromCopy.Child?.Id);
        }
    }
}
