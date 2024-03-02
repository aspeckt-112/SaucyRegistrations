using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Extensions;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the <see cref="Assemblies" /> class.
/// </summary>
internal class AssemblyBuilder
{
    /// <summary>
    /// Builds the <see cref="Assemblies" /> class.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation" />.</param>
    /// <returns>An instance of the <see cref="Assemblies" /> class.</returns>
    internal IList<IAssemblySymbol> Build(Compilation compilation)
    {
        var assemblies = new List<IAssemblySymbol>();

        IAssemblySymbol compilationAssembly = compilation.Assembly;

        if (compilationAssembly.ShouldBeIncludedInSourceGeneration())
        {
            assemblies.Add(compilationAssembly);
        }

        var referencedAssemblies = compilation.SourceModule.ReferencedAssemblySymbols.Where(x => x.ShouldBeIncludedInSourceGeneration()).ToList();

        assemblies.AddRange(referencedAssemblies);

        return assemblies;
    }
}