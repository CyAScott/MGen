using System;

namespace MGen
{
    /// <summary>
    /// Adds type conversion support to a class.
    /// </summary>
    public interface ISupportConversion : IConvertible
    {
        /// <summary>
        /// Gets the value by property name.
        /// </summary>
        bool TryGetValue(string name, out object? value);
    }
}
