using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Builders;
using SaucyRegistrations.Generators.Configurations;

namespace SaucyRegistrations.Generators.Collections;

/// <summary>
/// A collection of assemblies and their scan configurations.
/// </summary>
/// <remarks>Uses the <see cref="IAssemblySymbol" /> as the key.</remarks>
/// <seealso cref="AssemblyScanConfiguration" />
/// <seealso cref="IAssemblySymbol" />
internal class Assemblies : Dictionary<IAssemblySymbol, AssemblyScanConfiguration>
{
    /// <summary>
    /// Gets a value indicating whether the collection is empty.
    /// </summary>
    /// <value><c>true</c> if the collection is empty; otherwise, <c>false</c>.</value>
    internal bool IsEmpty => Count == 0;

    /// <summary>
    /// Adds an assembly to the collection with a configuration.
    /// </summary>
    /// <param name="assembly">The assembly to add.</param>
    /// <returns>An object allowing configuration of the scan for the added assembly.</returns>
    internal AssemblyConfigurationBuilder Add(IAssemblySymbol assembly)
    {
        return new AssemblyConfigurationBuilder(this, assembly);
    }
}