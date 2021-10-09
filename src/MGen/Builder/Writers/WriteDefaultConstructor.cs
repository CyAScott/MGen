using MGen.Builder.BuilderContext;
using System;
using System.Linq;

namespace MGen.Builder.Writers
{
    partial class WriteDefaultConstructor
    {
        public bool HasDefaultConstructor { get; set; }

        public static readonly WriteDefaultConstructor Instance = new();
    }

    partial class WriteDefaultConstructor : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            HasDefaultConstructor = false;

            next();

            context.Builder.AppendConstructor(context, new ConstructorBuilder());
        }
    }

    partial class WriteDefaultConstructor : IHandleBuildingConstructors
    {
        public void Handle(ConstructorBuilderContext context, Action next)
        {
            if (context.Constructor.Count == 0)
            {
                if (HasDefaultConstructor)
                {
                    return;
                }

                HasDefaultConstructor = true;
            }

            var constructor = context.Constructor;

            foreach (var line in constructor.XmlComments.SkipWhile(string.IsNullOrEmpty))
            {
                context.Builder.AppendLine(builder => builder.Append("/// ").Append(line));
            }

            var builder = context.Builder.AppendIndent().String
                .Append(constructor.Modifier).Append(' ').Append(context.ClassName).Append('(');
            for (var index = 0; index < constructor.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(constructor[index].FullLine);
            }
            context.Builder.AppendLine(")");

            context.Builder.OpenBrace();
            foreach (var action in constructor.Body)
            {
                action(context);
            }
            context.Builder.CloseBrace().AppendLine();
        }
    }
}
