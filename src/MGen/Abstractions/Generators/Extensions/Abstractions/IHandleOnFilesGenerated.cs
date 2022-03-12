using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnFilesGenerated : IAmAMGenExtension
{
    void FilesGenerated(FilesGeneratedArgs args);
}

[DebuggerStepThrough]
public class FilesGeneratedArgs
{
    public FilesGeneratedArgs(GeneratorContext context) => Context = context;

    public GeneratorContext Context { get; }
}