using System;
using System.Linq;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;

using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the generation configuration.
/// </summary>
internal static class GenerationConfigurationBuilder
{
    /// <summary>
    /// Builds the generation configuration.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <returns>
    /// An instance of the <see cref="GenerationConfiguration" /> class, or null if there's no generation
    /// configuration found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <see cref="ServiceCollectionMethod" /> is not applied to a
    /// class in the compilation assembly.
    /// </exception>
    internal static GenerationConfiguration? Build(Compilation compilation)
    {
        IAssemblySymbol compilationAssembly = compilation.Assembly;

        var compilationAssemblyNamespaces = compilationAssembly.GlobalNamespace.GetNamespaces().ToList();

        if (compilationAssemblyNamespaces.Count == 0)
        {
            return null;
        }

        GenerationConfiguration? generationConfiguration = null;

        // We need to find the first class with the ServiceCollectionMethod attribute.
        // For example, Program.cs in a console application. App.xaml.cs in a desktop application, etc.
        foreach (INamespaceSymbol? @namespace in compilationAssemblyNamespaces)
        {
            foreach (INamedTypeSymbol? namedTypeSymbol in @namespace.GetTypeMembers())
            {
                foreach (AttributeData? attribute in namedTypeSymbol.GetAttributes())
                {
                    if (attribute.Is<ServiceCollectionMethod>())
                    {
                        var methodName = attribute.GetValueForPropertyOfType<string>(nameof(ServiceCollectionMethod.MethodName));
                        generationConfiguration = new GenerationConfiguration(@namespace.ToDisplayString(), namedTypeSymbol.Name, methodName);
                    }
                }
            }
        }

        if (generationConfiguration is null)
        {
            throw new InvalidOperationException("No generation configuration found. Have you applied the [GenerateServiceCollectionMethod] attribute to a class?");
        }

        return generationConfiguration;
    }
}