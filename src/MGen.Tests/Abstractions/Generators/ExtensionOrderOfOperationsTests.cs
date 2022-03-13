using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

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
        diagnostics.ShouldBeEmpty();

        order.Count.ShouldBe(5);
        order[0].ShouldBe(nameof(testModelGenerator.Init));
        order[1].ShouldBe(nameof(testModelGenerator.TypeGenerated));
        order[2].ShouldBe(nameof(testModelGenerator.TypesGenerated));
        order[3].ShouldBe(nameof(testModelGenerator.FileGenerated));
        order[4].ShouldBe(nameof(testModelGenerator.FilesGenerated));

        foreach (var pair in argValues)
        {
            pair.Value.ShouldNotBeNull($"Arg for {pair.Key} is null.");
        }
    }
}