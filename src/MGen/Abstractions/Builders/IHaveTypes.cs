using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Builders;

public interface IHaveTypes : IHaveClasses, IHaveCodeGenerators, IHaveInterfaces, IHaveRecords, IHaveState, IHaveStructs
{
    string GetFullPath(bool includeSelf = true);
}
