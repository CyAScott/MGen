using Microsoft.CodeAnalysis;
using System;

namespace MGen.Builder.BuilderContext
{
    public class PropertyBuilderContext : MemberBuilderContext
    {
        internal PropertyBuilderContext(ClassBuilderContext context, IPropertySymbol primary, bool @explicit, IPropertySymbol? secondary = null, string? fieldName = null)
            : base(context, primary, @explicit)
        {
            FieldName = fieldName;
            Secondary = secondary;
        }

        /// <summary>
        /// The primary property that may only contain either a get or set method.
        /// </summary>
        public IPropertySymbol Primary => (IPropertySymbol)Member;

        /// <summary>
        /// The secondary property that will only contain either a get or set method.
        /// </summary>
        public IPropertySymbol? Secondary { get; }

        /// <summary>
        /// If the property requires a get method.
        /// </summary>
        public bool HasGet => Primary.GetMethod != null || Secondary?.GetMethod != null;

        /// <summary>
        /// If the property requires a set method.
        /// </summary>
        public bool HasSet => Primary.SetMethod != null || Secondary?.SetMethod != null;

        /// <summary>
        /// The private field for the property.
        /// </summary>
        public string? FieldName { get; }
    }

    public interface IHandleBuildingProperties : IAmAnExtension
    {
        public void Handle(PropertyBuilderContext context, Action next);
    }
}
