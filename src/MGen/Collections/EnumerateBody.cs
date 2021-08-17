namespace MGen.Collections
{
    /// <summary>
    /// A delegate for building a body within a loop for enumerating through a data structure.
    /// </summary>
    /// <param name="context">The context for building the class.</param>
    /// <param name="elementValue">The reference to the element value. This reference is readonly and cannot be assigned a different value.</param>
    /// <param name="indices">
    /// The indices for accessing the element.
    /// The indices can be used by the <see cref="CollectionGenerator.Get"/> and <see cref="CollectionGenerator.Set"/> methods to read and write the element value.
    /// </param>
    public delegate void EnumerateBody(CollectionGenerator context, string elementValue, params string[] indices);
}
