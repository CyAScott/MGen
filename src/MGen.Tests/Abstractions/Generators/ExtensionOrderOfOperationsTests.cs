using System.Collections.Generic;
using NUnit.Framework;

namespace MGen.Abstractions.Generators;

class ExtensionOrderOfOperationsTests
{
    [Test]
    public void TestOrderOfOperations()
    {
        var testModelGenerator = new TestModelGenerator(
            "using MGen;",
            "",
            "namespace Example;",
            "",
            "[Generate]",
            "interface IExample { }");

        var order = new List<string>();
        var argValues = new Dictionary<string, object>();

        testModelGenerator.Init += args =>
        {
            order.Add(nameof(testModelGenerator.Init));
            argValues.Add(nameof(testModelGenerator.Init), args);
        };

        testModelGenerator.FileGenerated += args =>
        {
            order.Add(nameof(testModelGenerator.FileGenerated));
            argValues.Add(nameof(testModelGenerator.FileGenerated), args);
        };

        testModelGenerator.FilesGenerated += args =>
        {
            order.Add(nameof(testModelGenerator.FilesGenerated));
            argValues.Add(nameof(testModelGenerator.FilesGenerated), args);
        };

        testModelGenerator.TypeGenerated += args =>
        {
            order.Add(nameof(testModelGenerator.TypeGenerated));
            argValues.Add(nameof(testModelGenerator.TypeGenerated), args);
        };

        testModelGenerator.TypesGenerated += args =>
        {
            order.Add(nameof(testModelGenerator.TypesGenerated));
            argValues.Add(nameof(testModelGenerator.TypesGenerated), args);
        };

        testModelGenerator.Compile(out var diagnostics);
        Assert.IsEmpty(diagnostics);

        Assert.AreEqual(5, order.Count);
        Assert.AreEqual(nameof(testModelGenerator.Init), order[0]);
        Assert.AreEqual(nameof(testModelGenerator.TypeGenerated), order[1]);
        Assert.AreEqual(nameof(testModelGenerator.TypesGenerated), order[2]);
        Assert.AreEqual(nameof(testModelGenerator.FileGenerated), order[3]);
        Assert.AreEqual(nameof(testModelGenerator.FilesGenerated), order[4]);

        foreach (var pair in argValues)
        {
            Assert.IsNotNull(pair.Value, $"Arg for {pair.Key} is null.");
        }
    }
}