using System.Collections.Generic;

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
    /// Builds the generation configuration from the given list of namespaces.
    /// </summary>
    /// <param name="namespaces">The list of namespaces to build the generation configuration from.</param>
    /// <returns>The generation configuration if found; otherwise, null.</returns>
    internal static GenerationConfiguration? Build(List<INamespaceSymbol> namespaces)
    {
        // Within each namespace, look for the first class with the ServiceCollectionMethod attribute.
        // If it's found, then use the namespace and class name to build the generation configuration.
        // If it's not found, then return null.
        foreach (INamespaceSymbol? @namespace in namespaces)
        {
            foreach (INamedTypeSymbol? namedTypeSymbol in @namespace.GetTypeMembers())
            {
                foreach (AttributeData? attribute in namedTypeSymbol.GetAttributes())
                {
                    if (attribute.Is<ServiceCollectionMethod>())
                    {
                        var methodName = attribute.GetValueOfPropertyWithName<string>(nameof(ServiceCollectionMethod.MethodName));

                        return new GenerationConfiguration(@namespace.ToDisplayString(), namedTypeSymbol.Name, methodName);
                    }
                }
            }
        }

        return null;
    }
}