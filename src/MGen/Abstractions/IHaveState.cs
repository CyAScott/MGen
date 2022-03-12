using System.Collections.Generic;

namespace MGen.Abstractions;

public interface IHaveState
{
    Dictionary<string, object> State { get; }
}