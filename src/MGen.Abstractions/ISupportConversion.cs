using System;

namespace MGen
{
    public interface ISupportConversion : IConvertible
    {
        /// <summary>
        /// Gets the value by property name.
        /// </summary>
        bool TryGetValue(string name, out object? value);
    }
}
