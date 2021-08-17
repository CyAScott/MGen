using Microsoft.CodeAnalysis;

namespace MGen.Builder.BuilderContext
{
    public class MemberBuilderContext : ClassBuilderContext
    {
        protected MemberBuilderContext(ClassBuilderContext context, ISymbol member, bool @explicit)
            : base(context)
        {
            Explicit = @explicit;
            Member = member;
        }

        /// <summary>
        /// The member that is being declared (i.e. property, method, or event).
        /// </summary>
        public ISymbol Member { get; }

        /// <summary>
        /// If the member should be explictlly defined (i.e. void IDisposable.Dispose() { } ).
        /// </summary>
        public bool Explicit { get; }
    }
}
