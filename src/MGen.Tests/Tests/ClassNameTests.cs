using NUnit.Framework;

namespace MGen.Tests
{
    [Generate(
        SourceNamePattern = @"^IHaveA(?<name>[A-Z]\w+)$",
        DestinationNamePattern = "{{name}}Class")]
    public interface IHaveACustomNamePattern
    {
    }

    public class ClassNameTests
    {
        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IHaveACustomNamePattern>();
            Assert.IsNotNull(type);
            Assert.AreEqual("CustomNamePatternClass", type.Name);
        }
    }
}
