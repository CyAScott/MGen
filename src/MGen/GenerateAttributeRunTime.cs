namespace MGen
{
    public sealed class GenerateAttributeRuntime
    {
        /// <summary>
        /// If true then the interface types for the properties in this interface will also get classes generated.
        /// </summary>
        public bool GenerateNestedClasses { get; set; } = true;

        /// <summary>
        /// The pattern for creating the name for the class.
        /// </summary>
        public string DestinationNamePattern { get; set; } = "{{interfaceName | name}}Model";

        /// <summary>
        /// The <see cref="System.Text.RegularExpressions.Regex"/> pattern for getting values from the interface name.
        /// </summary>
        public string SourceNamePattern { get; set; } = @"^(I(?<interfaceName>[A-Z]\w+)|(?<name>\w+))$";
    }
}
