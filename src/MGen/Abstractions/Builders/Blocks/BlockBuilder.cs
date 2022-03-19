using System.Diagnostics;
using System.Text;

namespace MGen.Abstractions.Builders.Blocks;

public static partial class CodeBlockExtensions
{
    [DebuggerStepThrough]
    public static BlockBuilder AddBlock(this BlockOfCodeBase parent) => parent.Add(new BlockBuilder(parent));
}

[DebuggerStepThrough]
public class BlockBuilder : BlockOfCode
{
    internal BlockBuilder(BlockOfCodeBase parent)
        : base(parent)
    {
    }

    protected override void AppendHeader(StringBuilder stringBuilder)
    {
    }
}
