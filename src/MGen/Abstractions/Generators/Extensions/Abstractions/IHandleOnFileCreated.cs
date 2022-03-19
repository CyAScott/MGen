using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnFileCreated : IAmAMGenExtension
{
    void FileCreated(FileCreatedArgs args);
}

[DebuggerStepThrough]
public class FileCreatedArgs
{
    public FileCreatedArgs(GeneratorContext context, FileGenerator generator)
    {
        Context = context;
        Generator = generator;
    }

    public GeneratorContext Context { get; }

    public FileGenerator Generator { get; }
}