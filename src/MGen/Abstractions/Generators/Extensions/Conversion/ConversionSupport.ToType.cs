using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.Conversion;

partial class ConversionCodeGenerator
{
    void GenerateToType(MethodCodeGenerationArgs args)
    {
        var method = args.Builder;

        var foreachLoop = method.AddForeachLoop("var", "constructor", "conversionType.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)");

        foreachLoop.AddVariable("var", "parameters", "constructor.GetParameters()");

        var @if = foreachLoop.AddIf("parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(MGen.ISupportConversion))");

        @if.Return("constructor.Invoke(new object[] { this })");

        method.AddLine("throw new System.InvalidCastException()");
    }
}