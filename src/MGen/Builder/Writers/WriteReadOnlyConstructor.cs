using MGen.Builder.BuilderContext;
using System;

namespace MGen.Builder.Writers
{
    partial class WriteReadOnlyConstructor
    {
        public ConstructorBuilder ReadOnlyConstructor { get; } = new();

        public static readonly WriteReadOnlyConstructor Instance = new();
    }

    partial class WriteReadOnlyConstructor : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            ReadOnlyConstructor.Reset();

            next();

            if (ReadOnlyConstructor.Count == 0)
            {
                return;
            }

            var comments = context.Interface.GetDocumentationCommentXml();

            if (!string.IsNullOrEmpty(comments))
            {
                var lines = comments?.Split('\n') ?? Array.Empty<string>();

                foreach (var rawLine in lines)
                {
                    var line = rawLine.TrimStart().TrimEnd('\r');
                    if (!string.IsNullOrEmpty(line) &&
                        !rawLine.StartsWith("<member name=") &&
                        !rawLine.StartsWith("</member>"))
                    {
                        ReadOnlyConstructor.XmlComments.Add(line);
                    }
                }
            }

            context.Builder.AppendConstructor(context, ReadOnlyConstructor);
        }
    }

    partial class WriteReadOnlyConstructor : IHandleBuildingConstructors
    {
        public void Handle(ConstructorBuilderContext context, Action next)
        {
            if (ReadOnlyConstructor.Count == 0 || context.Constructor.Count > 0)
            {
                next();
            }
        }
    }

    partial class WriteReadOnlyConstructor : IHandleBuildingProperties
    {
        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();

            if (!context.Explicit && !context.HasSet && context.FieldName != null)
            {
                ReadOnlyConstructor.Add(new ConstructorParameter(context.Primary.Type, context.Primary.Name, $"{context.Primary.Type.ToCsString()} {context.Primary.Name}"));
                ReadOnlyConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder.Append(context.FieldName).Append(" = ").Append(context.Primary.Name).Append(';')));
            }
        }
    }
}
