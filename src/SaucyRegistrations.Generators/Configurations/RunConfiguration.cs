using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Configurations;

/// <summary>
/// The configuration for the generation process.
/// </summary>
/// <param name="generationConfiguration">An instance of the <see cref="GenerationConfiguration" /> class.</param>
/// <param name="typesToRegister">A dictionary of types to register and their service scope.</param>
public class RunConfiguration(GenerationConfiguration generationConfiguration, Dictionary<ITypeSymbol, ServiceScope> typesToRegister)
{
    /// <summary>
    /// Gets the generation configuration.
    /// </summary>
    public GenerationConfiguration GenerationConfiguration { get; } = generationConfiguration;

    /// <summary>
    /// Gets the types to register and their service scope.
    /// </summary>
    public Dictionary<ITypeSymbol, ServiceScope> TypesToRegister { get; } = typesToRegister;
}