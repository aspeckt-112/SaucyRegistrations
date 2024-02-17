using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Collections;
using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the <see cref="Assemblies"/> class.
/// </summary>
internal static class AssemblyCollectionBuilder
{
    /// <summary>
    /// Builds the <see cref="Assemblies"/> class.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/>.</param>
    /// <returns>An instance of the <see cref="Assemblies"/> class.</returns>
    internal static Assemblies Build(Compilation compilation)
    {
        var assemblyCollection = new Assemblies();

        IAssemblySymbol compilationAssembly = compilation.Assembly;

        // I think I hate how the service scope is being passed around here.
        if (compilationAssembly.ShouldBeIncludedInSourceGeneration(out ServiceScope? compilationServiceScope))
        {
            AssemblyScanConfiguration assemblyScanConfiguration = AssemblyScanConfigurationBuilder.Build(compilationAssembly, compilationServiceScope!);
            assemblyCollection.Add(compilationAssembly).WithConfiguration(assemblyScanConfiguration);
        }

        // This is a big bag of crap. Should be refactored once I'm happy with everything else.
        List<(IAssemblySymbol? assembly, ServiceScope? serviceScope)> referencedAssemblies = compilation.SourceModule
                                                                                                        .ReferencedAssemblySymbols
                                                                                                        .Select(
                                                                                                            a => a.ShouldBeIncludedInSourceGeneration(
                                                                                                                out ServiceScope? serviceScope
                                                                                                            )
                                                                                                                ? (a, serviceScope)
                                                                                                                : (null, null))
                                                                                                        .Where(x => x.Item1 is not null)
                                                                                                        .ToList();

        foreach ((IAssemblySymbol? assembly, ServiceScope? referencedServiceScope) in referencedAssemblies)
        {
            AssemblyScanConfiguration assemblyScanConfiguration = AssemblyScanConfigurationBuilder.Build(assembly!, referencedServiceScope!);
            assemblyCollection.Add(assembly!).WithConfiguration(assemblyScanConfiguration);
        }

        return assemblyCollection;
    }
}