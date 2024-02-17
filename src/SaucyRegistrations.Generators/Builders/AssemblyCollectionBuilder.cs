using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Collections;
using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Logging;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the <see cref="Assemblies" /> class.
/// </summary>
internal class AssemblyCollectionBuilder
{
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyCollectionBuilder" /> class.
    /// </summary>
    /// <param name="logger">The <see cref="Logger"/>.</param>
    internal AssemblyCollectionBuilder(Logger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Builds the <see cref="Assemblies" /> class.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation" />.</param>
    /// <returns>An instance of the <see cref="Assemblies" /> class.</returns>
    internal Assemblies Build(Compilation compilation)
    {
        var assemblyCollection = new Assemblies();

        IAssemblySymbol compilationAssembly = compilation.Assembly;

        _logger.WriteInformation($"Building assembly collection for assembly: {compilationAssembly.Name}");
        _logger.WriteInformation($"Checking if {compilationAssembly.Name} should be included in source generation...");

        // I think I hate how the service scope is being passed around here.
        if (compilationAssembly.ShouldBeIncludedInSourceGeneration(out ServiceScope? compilationServiceScope))
        {
            _logger.WriteInformation($"{compilationAssembly.Name} should be included in source generation.");
            _logger.WriteInformation($"Building assembly scan configuration for {compilationAssembly.Name}...");
            AssemblyScanConfiguration assemblyScanConfiguration = AssemblyScanConfigurationBuilder.Build(compilationAssembly, compilationServiceScope!);
            assemblyCollection.Add(compilationAssembly).WithConfiguration(assemblyScanConfiguration);
        }

        _logger.WriteInformation($"Checking referenced assemblies for {compilationAssembly.Name}...");

        // This is a big bag of crap. Should be refactored once I'm happy with everything else.
        List<(IAssemblySymbol? assembly, ServiceScope? serviceScope)> referencedAssemblies = compilation.SourceModule
                                                                                                        .ReferencedAssemblySymbols
                                                                                                        .Select(
                                                                                                            a => a.ShouldBeIncludedInSourceGeneration(
                                                                                                                out ServiceScope? serviceScope
                                                                                                            )
                                                                                                                ? (a, serviceScope)
                                                                                                                : (null, null)
                                                                                                        )
                                                                                                        .Where(x => x.Item1 is not null)
                                                                                                        .ToList();

        _logger.WriteInformation($"Found {referencedAssemblies.Count} referenced assemblies for {compilationAssembly.Name}...");

        foreach ((IAssemblySymbol? assembly, ServiceScope? referencedServiceScope) in referencedAssemblies)
        {
            _logger.WriteInformation($"Building assembly scan configuration for {assembly!.Name}...");
            AssemblyScanConfiguration assemblyScanConfiguration = AssemblyScanConfigurationBuilder.Build(assembly!, referencedServiceScope!);
            assemblyCollection.Add(assembly!).WithConfiguration(assemblyScanConfiguration);
        }

        return assemblyCollection;
    }
}