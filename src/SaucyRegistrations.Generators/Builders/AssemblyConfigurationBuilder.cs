using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Collections;
using SaucyRegistrations.Generators.Configurations;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the assembly configuration.
/// </summary>
internal class AssemblyConfigurationBuilder
{
    private readonly Assemblies _assemblies;
    private readonly IAssemblySymbol _assembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyConfigurationBuilder" /> class.
    /// </summary>
    /// <param name="assemblies">The assembly collection.</param>
    /// <param name="assembly">The assembly.</param>
    internal AssemblyConfigurationBuilder(Assemblies assemblies, IAssemblySymbol assembly)
    {
        _assemblies = assemblies;
        _assembly = assembly;
    }

    /// <summary>
    /// Sets the scan configuration for the assembly.
    /// </summary>
    /// <param name="configuration">The scan configuration for the assembly.</param>
    public void WithConfiguration(AssemblyScanConfiguration configuration)
    {
        _assemblies.Add(_assembly, configuration);
    }
}