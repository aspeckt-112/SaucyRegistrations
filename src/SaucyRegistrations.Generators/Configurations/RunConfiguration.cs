using SaucyRegistrations.Generators.Collections;

namespace SaucyRegistrations.Generators.Configurations;

/// <summary>
/// The configuration for the generation process.
/// </summary>
/// <param name="generationConfiguration">An instance of the <see cref="GenerationConfiguration" /> class.</param>
/// <param name="typesToRegister">An instance of the <see cref="TypeSymbols" /> class.</param>
internal class RunConfiguration(GenerationConfiguration generationConfiguration, TypeSymbols typesToRegister)
{
    /// <summary>
    /// Gets the generation configuration.
    /// </summary>
    internal GenerationConfiguration GenerationConfiguration { get; } = generationConfiguration;

    /// <summary>
    /// Gets the types to register and their service scope.
    /// </summary>
    internal TypeSymbols TypesToRegister { get; } = typesToRegister;
}