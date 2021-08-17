using MGen.Builder.BuilderContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGen.Builder.Writers
{
    partial class WriteNetSerialization
    {
        public WriteNetSerialization()
        {
            DeserializeConstructor.XmlComments.Add("<summary>");
            DeserializeConstructor.XmlComments.Add($"Deserializes the class.");
            DeserializeConstructor.XmlComments.Add("</summary>");
            DeserializeConstructor.Modifier = "protected";
            DeserializeConstructor.Body.Add(ctx => ctx.Builder
                .AppendLine("if (info == null) throw new System.ArgumentNullException(\"info\");")
                .AppendLine("if (context == null) throw new System.ArgumentNullException(\"context\");")
                .AppendLine());
        }

        public ConstructorBuilder DeserializeConstructor { get; } = new();

        public List<string> SerializerBody { get; } = new();

        public bool SupportsSerialization { get; set; }

        public static readonly WriteNetSerialization Instance = new();
    }

    partial class WriteNetSerialization : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            if (DeserializeConstructor.Body.Count > 1)
            {
                DeserializeConstructor.Body.RemoveRange(1, DeserializeConstructor.Body.Count - 1);
            }

            SerializerBody.Clear();
            SupportsSerialization = false;

            foreach (var inheritInterface in context.Interface.AllInterfaces)
            {
                if (inheritInterface.ContainingAssembly.Name == "System.Runtime" &&
                    inheritInterface.ContainingNamespace.Name == "Serialization" &&
                    inheritInterface.Name == "ISerializable")
                {
                    if (DeserializeConstructor.Count == 0)
                    {
                        var serializationMethod = (IMethodSymbol)inheritInterface.GetMembers().Single();

                        DeserializeConstructor.Add(new ConstructorParameter(serializationMethod.Parameters[0].Type, "info", "System.Runtime.Serialization.SerializationInfo info"));
                        DeserializeConstructor.Add(new ConstructorParameter(serializationMethod.Parameters[1].Type, "context", "System.Runtime.Serialization.StreamingContext context"));
                    }

                    SupportsSerialization = true;

                    break;
                }
            }

            next();

            if (!SupportsSerialization)
            {
                return;
            }

            context.Builder.AppendConstructor(context, DeserializeConstructor);


            context.Builder.AppendLine("void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)");
            context.Builder.OpenBrace();

            context.Builder.AppendLine("if (info == null) throw new System.ArgumentNullException(\"info\");");
            context.Builder.AppendLine("if (context == null) throw new System.ArgumentNullException(\"context\");");

            foreach (var line in SerializerBody)
            {
                context.Builder.AppendLine(line);
            }

            context.Builder.CloseBrace();
        }
    }

    partial class WriteNetSerialization : IHandleBuildingMethods
    {
        public void Handle(MethodBuilderContext context, Action next)
        {
            if (context.Explicit ||
                !SupportsSerialization ||
                context.Method.Name != "GetObjectData" ||
                context.Method.ContainingSymbol.ContainingAssembly.Name != "System.Runtime" ||
                context.Method.ContainingSymbol.ContainingNamespace.Name != "Serialization" ||
                context.Method.ContainingSymbol.ContainingSymbol.Name != "ISerializable")
            {
                next();
            }
        }
    }

    partial class WriteNetSerialization : IHandleBuildingProperties
    {
        public void Handle(PropertyBuilderContext context, Action next)
        {
            next();

            if (!SupportsSerialization || context.FieldName == null)
            {
                return;
            }

            //todo: serialize / deserialize collections and arrays

            DeserializeConstructor.Body.Add(ctx => ctx.Builder.AppendLine(builder => builder
                .Append(context.FieldName).Append(" = (").AppendType(context.Primary.Type).Append(")info.GetValue(\"").Append(context.Primary.Name).Append("\", typeof(").AppendType(context.Primary.Type).Append("));")));
            SerializerBody.Add($"info.AddValue(\"{context.Primary.Name}\", {context.FieldName}, typeof({context.Primary.Type}));");
        }
    }
}
