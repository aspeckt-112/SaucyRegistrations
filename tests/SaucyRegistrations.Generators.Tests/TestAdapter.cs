using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Tests;

// https://github.com/jmarolf/generator-start/blob/main/tests/Adapter.cs

public class TestAdapter<TIncrementalGenerator> : ISourceGenerator, IIncrementalGenerator
    where TIncrementalGenerator : IIncrementalGenerator, new()
{
    private readonly TIncrementalGenerator _internalGenerator = new();

    public void Execute(GeneratorExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _internalGenerator.Initialize(context);
    }
}