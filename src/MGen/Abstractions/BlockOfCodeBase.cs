using System.Diagnostics;
using MGen.Abstractions.Generators.Extensions.Abstractions;

namespace MGen.Abstractions;

[DebuggerStepThrough]
public abstract class BlockOfCodeBase : CodeCollection, IHaveCodeGenerators
{
    protected BlockOfCodeBase(CodeGenerators codeGenerators, int indentLevel)
        : base(indentLevel) =>
        CodeGenerators = codeGenerators;

    public CodeGenerators CodeGenerators { get; }
}

[DebuggerStepThrough]
public abstract class BlockOfCode<TParent> : BlockOfCodeBase
    where TParent : IAmIndentedCode, IHaveCodeGenerators
{
    protected BlockOfCode(TParent parent, int? indentLevel = null)
        : base(parent.CodeGenerators, indentLevel ?? parent.IndentLevel + 1) =>
        Parent = parent;

    public TParent Parent { get; }
}

[DebuggerStepThrough]
public abstract class BlockOfCode : BlockOfCode<BlockOfCodeBase>
{
    protected BlockOfCode(BlockOfCodeBase parent, int? indentLevel = null)
        : base(parent, indentLevel)
    {
    }
}
