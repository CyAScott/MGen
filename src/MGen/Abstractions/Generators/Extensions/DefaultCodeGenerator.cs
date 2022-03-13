using MGen.Abstractions.Builders.Blocks;
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
            args.Builder.AddLine("throw new System.NotImplementedException()");
        }
    }

    public void Handle(PropertyGetCodeGenerationArgs args)
    {
        if (args.Builder.ArgumentsEnabled)
        {
            args.Builder.Get.AddLine("throw new System.NotImplementedException()");
        }
        else if (args.Builder.Field != null)
        {
            args.Builder.Get.Return(args.Builder.Field.Name);
        }
    }

    public void Handle(PropertySetCodeGenerationArgs args)
    {
        if (args.Builder.ArgumentsEnabled)
        {
            args.Builder.Set.AddLine("throw new System.NotImplementedException()");
        }
        else if (args.Builder.Field != null)
        {
            args.Builder.Set.Set(args.Builder.Field.Name, "value");
        }
    }
}