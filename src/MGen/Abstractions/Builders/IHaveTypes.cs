using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Builders;

public interface IHaveTypes : IHaveClasses, IHaveInterfaces, IHaveRecords, IHaveState, IHaveStructs
{
    HandlerCollection Handlers { get; }
}