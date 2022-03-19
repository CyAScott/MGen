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

        testModelGenerator.FileCreated += args =>
        {
            order.Add(nameof(testModelGenerator.FileCreated));
            argValues.Add(nameof(testModelGenerator.FileCreated), args);
        };

        testModelGenerator.FilesCreated += args =>
        {
            order.Add(nameof(testModelGenerator.FilesCreated));
            argValues.Add(nameof(testModelGenerator.FilesCreated), args);
        };

        testModelGenerator.TypeGenerated += args =>
        {
            order.Add(nameof(testModelGenerator.TypeGenerated));
            argValues.Add(nameof(testModelGenerator.TypeGenerated), args);
        };

        testModelGenerator.Compile().EmitResult.Diagnostics.ShouldBeEmpty();

        order.Count.ShouldBe(6);
        order[0].ShouldBe(nameof(testModelGenerator.Init));
        order[1].ShouldBe(nameof(testModelGenerator.TypeGenerated));
        order[2].ShouldBe(nameof(testModelGenerator.FileCreated));
        order[3].ShouldBe(nameof(testModelGenerator.FilesCreated));
        order[4].ShouldBe(nameof(testModelGenerator.FileGenerated));
        order[5].ShouldBe(nameof(testModelGenerator.FilesGenerated));

        foreach (var pair in argValues)
        {
            pair.Value.ShouldNotBeNull($"Arg for {pair.Key} is null.");
        }
    }
}