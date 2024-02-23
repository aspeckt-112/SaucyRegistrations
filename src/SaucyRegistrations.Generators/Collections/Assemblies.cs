// using System.Collections.Generic;
//
// using Microsoft.CodeAnalysis;
//
// using SaucyRegistrations.Generators.Builders;
// using SaucyRegistrations.Generators.Configurations;
//
// namespace SaucyRegistrations.Generators.Collections;
//
// /// <summary>
// /// A collection of assemblies and their scan configurations.
// /// </summary>
// /// <remarks>Uses the <see cref="IAssemblySymbol" /> as the key.</remarks>
// /// <seealso cref="AssemblyScanConfiguration" />
// /// <seealso cref="IAssemblySymbol" />
// internal class Assemblies : Dictionary<IAssemblySymbol, AssemblyScanConfiguration>
// {
//     /// <summary>
//     /// Adds an assembly to the collection with a configuration.
//     /// </summary>
//     /// <param name="assembly">The assembly to add.</param>
//     /// <returns>The assembly configuration builder.</returns>
//     internal AssemblyConfigurationBuilder Add(IAssemblySymbol assembly)
//     {
//         return new AssemblyConfigurationBuilder(this, assembly);
//     }
// }
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

internal sealed class Assemblies : List<IAssemblySymbol>;