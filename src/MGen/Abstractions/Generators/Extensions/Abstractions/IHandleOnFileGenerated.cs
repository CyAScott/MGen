using System.Diagnostics;

namespace MGen.Abstractions.Generators.Extensions.Abstractions;

public interface IHandleOnFileGenerated : IAmAMGenExtension
{
    void FileGenerated(FileGeneratedArgs args);
}

[DebuggerStepThrough]
public class FileGeneratedArgs
{
    public FileGeneratedArgs(GeneratorContext context, FileGenerator generator, string contents)
    {
        Context = context;
        Generator = generator;
        Contents = contents;
    }

    public GeneratorContext Context { get; }
    public FileGenerator Generator { get; }
    public string Contents { get; }
}