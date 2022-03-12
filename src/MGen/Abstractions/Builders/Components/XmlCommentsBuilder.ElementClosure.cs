namespace MGen.Abstractions.Builders.Components;

public partial class XmlCommentsBuilder
{
    internal enum ElementClosure
    {
        None,
        StartOfElement,
        StartAndEndOfElement,
        EndOfElement
    }
}