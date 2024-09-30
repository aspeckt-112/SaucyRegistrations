using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Models;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamespaceSymbol" /> class.
/// </summary>
internal static class NamespaceSymbolExtensions
{
    /// <summary>
    /// Gets all descendant namespaces for the namespace symbol.
    /// </summary>
    /// <param name="namespace">The namespace symbol.</param>
    /// <returns>All descendant namespaces for the namespace symbol.</returns>
    internal static IEnumerable<INamespaceSymbol> GetDescendantNamespaces(this INamespaceSymbol @namespace)
    {
        foreach (INamespaceSymbol? symbol in @namespace.GetNamespaceMembers())
        {
            yield return symbol;

            foreach (INamespaceSymbol? childNamespace in GetDescendantNamespaces(symbol))
            {
                yield return childNamespace;
            }
        }
    }

    /// <summary>
    /// Gets the instantiable types in the namespace.
    /// </summary>
    /// <param name="namespace">The namespace symbol.</param>
    /// <returns>The instantiable types in the namespace.</returns>
    internal static ImmutableList<INamedTypeSymbol> GetInstantiableTypes(this INamespaceSymbol @namespace)
    {
        return @namespace
            .GetTypeMembers()
            .Where(symbol => !symbol.IsAbstract && !symbol.IsStatic)
            .ToImmutableList();
    }
}