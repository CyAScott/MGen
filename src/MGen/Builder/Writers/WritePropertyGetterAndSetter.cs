using MGen.Builder.BuilderContext;
using System;

namespace MGen.Builder.Writers
{
    partial class WritePropertyDefaultGetterAndSetter
    {
        public static readonly WritePropertyDefaultGetterAndSetter Instance = new();
    }

    partial class WritePropertyDefaultGetterAndSetter : IHandleBuildingPropertyGetters
    {
        public void Handle(PropertyGetterBuilderContext context, Action next)
        {
            if (context.Primary.IsIndexer || context.FieldName == null)
            {
                context.Builder.AppendLine("throw new System.NotImplementedException();");
            }
            else
            {
                context.Builder.AppendLine(builder => builder.Append("return ").Append(context.FieldName).Append(";"));
            }
        }
    }

    partial class WritePropertyDefaultGetterAndSetter : IHandleBuildingPropertySetters
    {
        public void Handle(PropertySetterBuilderContext context, Action next)
        {
            if (context.Primary.IsIndexer || context.FieldName == null)
            {
                context.Builder.AppendLine("throw new System.NotImplementedException();");
            }
            else
            {
                context.Builder.AppendLine(builder => builder.Append(context.FieldName).Append(" = value;"));
            }
        }
    }
}
