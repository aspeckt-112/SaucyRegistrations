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
internal class AssemblyBuilder
{
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyBuilder" /> class.
    /// </summary>
    /// <param name="logger">The <see cref="Logger"/>.</param>
    internal AssemblyBuilder(Logger logger)
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
        var assemblies = new Assemblies();

        IAssemblySymbol compilationAssembly = compilation.Assembly;

        _logger.WriteInformation($"Building assembly collection for assembly: {compilationAssembly.Name}");
        _logger.WriteInformation($"Checking if {compilationAssembly.Name} should be included in source generation...");

        // I think I hate how the service scope is being passed around here.
        if (compilationAssembly.ShouldBeIncludedInSourceGeneration())
        {
            assemblies.Add(compilationAssembly);
        }

        _logger.WriteInformation($"Checking referenced assemblies for {compilationAssembly.Name}...");

        // This is a big bag of crap. Should be refactored once I'm happy with everything else.
        List<IAssemblySymbol> referencedAssemblies = compilation.SourceModule
                                                                                                        .ReferencedAssemblySymbols
                                                                                                        .Where(x => x.ShouldBeIncludedInSourceGeneration())
                                                                                                        .ToList();

        _logger.WriteInformation($"Found {referencedAssemblies.Count} referenced assemblies for {compilationAssembly.Name}...");

        foreach (IAssemblySymbol referencedAssembly in referencedAssemblies)
        {
            assemblies.Add(referencedAssembly);
        }

        return assemblies;
    }
}