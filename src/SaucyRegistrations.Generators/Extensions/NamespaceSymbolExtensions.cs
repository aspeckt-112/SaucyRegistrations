using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamespaceSymbol" /> class.
/// </summary>
internal static class NamespaceSymbolExtensions
{
    /// <summary>
    /// Gets all nested namespaces for the namespace symbol.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol.</param>
    /// <returns>All nested namespaces for the namespace symbol.</returns>
    internal static IEnumerable<INamespaceSymbol> GetAllNestedNamespaces(this INamespaceSymbol namespaceSymbol)
    {
        foreach (INamespaceSymbol? symbol in namespaceSymbol.GetNamespaceMembers())
        {
            yield return symbol;

            foreach (INamespaceSymbol? childNamespace in GetAllNestedNamespaces(symbol))
            {
                yield return childNamespace;
            }
        }
    }

    /// <summary>
    /// Gets the instantiable types for the namespace symbol.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol.</param>
    /// <returns>The instantiable types for the namespace symbol.</returns>
    internal static ImmutableList<INamedTypeSymbol> GetInstantiableTypes(this INamespaceSymbol namespaceSymbol)
    {
        List<INamedTypeSymbol> types = [];
        types.AddRange(namespaceSymbol.GetTypeMembers().Where(symbol => !symbol.IsAbstract && !symbol.IsStatic));

        return types.ToImmutableList();
    }
}