using System.Diagnostics;

namespace MGen.Abstractions;

[DebuggerStepThrough]
public abstract class BlockOfCodeBase : CodeCollection
{
    protected BlockOfCodeBase(int indentLevel)
        : base(indentLevel)
    {
    }
}

[DebuggerStepThrough]
public abstract class BlockOfCode<TParent> : BlockOfCodeBase
    where TParent : IAmIndentedCode
{
    protected BlockOfCode(TParent parent, int? indentLevel = null)
        : base(indentLevel ?? parent.IndentLevel + 1) =>
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
