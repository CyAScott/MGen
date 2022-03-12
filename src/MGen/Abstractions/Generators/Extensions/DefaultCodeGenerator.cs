using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions;

[MGenExtension(Id)]
public class DefaultCodeGenerator : IHandleMethodCodeGeneration, IHandlePropertyGetCodeGeneration, IHandlePropertySetCodeGeneration
{
    public bool Enabled { get; set; } = true;

    public const string Id = "MGen." + nameof(DefaultCodeGenerator);

    public void Handle(MethodCodeGenerationArgs args)
    {
        if (args.Builder.IsBodyEnabled)
        {
            args.Builder.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 1)
                .AppendLine("throw new System.NotImplementedException();")));
        }
    }

    public void Handle(PropertyGetCodeGenerationArgs args)
    {
        if (args.Builder.ArgumentsEnabled)
        {
            args.Builder.Get.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 2)
                .AppendLine("throw new System.NotImplementedException();")));
        }
        else if (args.Builder.Field != null)
        {
            args.Builder.Get.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 2)
                .Append("return ")
                .Append(args.Builder.Field.Name)
                .AppendLine(";")));
        }
    }

    public void Handle(PropertySetCodeGenerationArgs args)
    {
        if (args.Builder.ArgumentsEnabled)
        {
            args.Builder.Set.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 2)
                .AppendLine("throw new System.NotImplementedException();")));
        }
        else if (args.Builder.Field != null)
        {
            args.Builder.Set.Add(new Code(stringBuilder => stringBuilder
                .AppendIndent(args.Builder.IndentLevel + 2)
                .Append(args.Builder.Field.Name)
                .AppendLine(" = value;")));
        }
    }
}