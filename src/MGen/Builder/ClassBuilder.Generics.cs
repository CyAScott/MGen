using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MGen.Builder
{
    public partial interface IClassBuilder
    {
        /// <summary>
        /// Appends the generic type names (i.e. &lt;TKey, TValue&gt;)
        /// </summary>
        IClassBuilder AppendGenericNames(ImmutableArray<ITypeSymbol>? genericArguments);

        /// <summary>
        /// Appends the generic constraints (i.e. where TKey : struct)
        /// </summary>
        IClassBuilder AppendGenericConstraints(ImmutableArray<ITypeSymbol>? genericArguments);
    }

    partial class ClassBuilder
    {
        public IClassBuilder AppendGenericNames(ImmutableArray<ITypeSymbol>? genericArguments)
        {
            if (genericArguments == null || genericArguments.Value.Length == 0)
            {
                return this;
            }

            var builder = String;

            builder.Append('<');

            for (var index = 0; index < genericArguments.Value.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(genericArguments.Value[index]);
            }

            builder.Append('>');

            return this;
        }

        public IClassBuilder AppendGenericConstraints(ImmutableArray<ITypeSymbol>? genericArguments)
        {
            if (genericArguments == null || genericArguments.Value.Length == 0)
            {
                return this;
            }

            IndentLevel++;

            foreach (var type in genericArguments.Value)
            {
                if (type is ITypeParameterSymbol typeParameter)
                {
                    AppendGenericConstraint(typeParameter);
                }
            }

            IndentLevel--;

            return this;
        }

        protected void AppendGenericConstraint(ITypeParameterSymbol typeParameter)
        {
            if (typeParameter.ConstraintNullableAnnotations.Length == 0 &&
                typeParameter.ConstraintTypes.Length == 0 &&
                !typeParameter.HasConstructorConstraint &&
                !typeParameter.HasNotNullConstraint &&
                !typeParameter.HasReferenceTypeConstraint &&
                !typeParameter.HasValueTypeConstraint &&
                typeParameter.ReferenceTypeConstraintNullableAnnotation != NullableAnnotation.Annotated)
            {
                return;
            }

            var builder = AppendIndent().String;

            builder.Append("where ").Append(typeParameter.Name).Append(" :");

            var hasParts = TryToAppendPointerConstraint(typeParameter);

            if (typeParameter.ConstraintTypes.Length > 0)
            {
                if (hasParts)
                {
                    builder.Append(',');
                }

                AppendTypeConstraints(typeParameter);

                hasParts = true;
            }

            if (typeParameter.HasConstructorConstraint)
            {
                if (hasParts)
                {
                    builder.Append(',');
                }
                builder.Append(" new()");
            }

            AppendLine();
        }

        protected bool TryToAppendPointerConstraint(ITypeParameterSymbol typeParameter)
        {
            var builder = String;

            if (typeParameter.HasReferenceTypeConstraint)
            {
                builder.Append(" class");

                if (typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated)
                {
                    builder.Append('?');
                }

                return true;
            }
            
            if (typeParameter.HasValueTypeConstraint)
            {
                builder.Append(" struct");
                return true;
            }

            if (typeParameter.HasNotNullConstraint)
            {
                builder.Append(" notnull");
                return true;
            }

            return false;
        }

        protected void AppendTypeConstraint(ITypeSymbol typeSymbol) =>
            String.Append(' ').AppendType(typeSymbol);

        protected void AppendTypeConstraints(ITypeParameterSymbol typeParameter)
        {
            for (var index = 0; index < typeParameter.ConstraintTypes.Length; index++)
            {
                if (index > 0)
                {
                    String.Append(',');
                }

                AppendTypeConstraint(typeParameter.ConstraintTypes[index]);
            }
        }
    }
}
