using System.Linq;
using MGen.Abstractions.Builders.Blocks;
using MGen.Abstractions.Builders.Members;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions.Generators.Extensions.DotNetSerialization;

partial class DotNetSerializationCodeGenerator : IHandleConstructorCodeGeneration
{
    public void Handle(ConstructorCodeGenerationArgs args)
    {
        if (!args.Builder.State.ContainsKey(DotNetSerializationSupport.InterfaceName) ||
            args.Builder.Parent is not IHaveProperties parent)
        {
            return;
        }

        args.Handled = true;

        var ctor = args.Builder;

        ctor.AddLine("if (info == null) throw new System.ArgumentNullException(\"info\")");
        ctor.AddEmptyLine();

        var @foreach = ctor.AddForeachLoop("it", "info");

        var @switch = @foreach.AddSwitchCase("it.Name");

        foreach (var property in parent.OfType<PropertyBuilder>())
        {
            if (property.Enabled && !property.ExplicitDeclaration.IsExplicitDeclarationEnabled &&
                property.ReturnType is CodeType codeType &&
                property.State.ContainsKey(DotNetSerializationSupport.InterfaceName))
            {
                var name = property.Field?.Name ?? property.Name;
                var type = codeType.Type;

                var @case = @switch.Cases.Add(new(sb => sb.Append('"').Append(property.Name).Append('"')));
                @case.Set(name, new(sb => sb.Append('(').AppendType(type).Append(')').Append("it.Value")));
            }
        }

        @foreach.Enabled = @switch.Cases.Count > 0;
    }
}