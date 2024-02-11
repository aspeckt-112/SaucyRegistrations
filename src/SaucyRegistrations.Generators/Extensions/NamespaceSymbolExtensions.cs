using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The extensions for the <see cref="INamespaceSymbol" /> type.
/// </summary>
internal static class NamespaceSymbolExtensions
{
    /// <summary>
    /// Gets a list of namespaces from the given namespace symbol.
    /// </summary>
    /// <remarks>Uses recursion to get all namespaces.</remarks>
    /// <param name="namespaceSymbol">The namespace symbol to get the list of namespaces from.</param>
    /// <returns>A list of namespaces.</returns>
    internal static IEnumerable<INamespaceSymbol> GetListOfNamespaces(this INamespaceSymbol namespaceSymbol)
    {
        foreach (INamespaceSymbol? symbol in namespaceSymbol.GetNamespaceMembers())
        {
            yield return symbol;

            foreach (INamespaceSymbol? childNamespace in GetListOfNamespaces(symbol))
            {
                yield return childNamespace;
            }
        }
    }
}