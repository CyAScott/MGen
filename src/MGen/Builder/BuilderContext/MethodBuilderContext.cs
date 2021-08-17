using Microsoft.CodeAnalysis;
using System;

namespace MGen.Builder.BuilderContext
{
    public class MethodBuilderContext : MemberBuilderContext
    {
        internal MethodBuilderContext(ClassBuilderContext context, IMethodSymbol method, bool @explicit)
            : base(context, method, @explicit)
        {
        }

        /// <summary>
        /// The method that is being written.
        /// </summary>
        public IMethodSymbol Method => (IMethodSymbol)Member;
    }

    public interface IHandleBuildingMethods
    {
        public void Handle(MethodBuilderContext context, Action next);
    }
}
