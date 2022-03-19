using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnFilesCreated : IAmAMGenExtension
{
    void FilesCreated(FilesCreatedArgs args);
}

[DebuggerStepThrough]
public class FilesCreatedArgs
{
    public FilesCreatedArgs(GeneratorContext context) => Context = context;

    public GeneratorContext Context { get; }
}