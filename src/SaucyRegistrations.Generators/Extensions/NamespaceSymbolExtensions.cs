using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The extensions for the <see cref="INamespaceSymbol" /> type.
/// </summary>
internal static class NamespaceSymbolExtensions
{
    /// <summary>
    /// Gets an enumerable list of namespaces from the given namespace symbol.
    /// </summary>
    /// <remarks>Uses recursion to get all namespaces.</remarks>
    /// <param name="namespaceSymbol">The <see cref="INamespaceSymbol"/> to get the list of namespaces from.</param>
    /// <returns>A <see cref="IEnumerable{T}" /> of <see cref="INamespaceSymbol" />.</returns>
    internal static IEnumerable<INamespaceSymbol> GetNamespaces(this INamespaceSymbol namespaceSymbol)
    {
        foreach (INamespaceSymbol? symbol in namespaceSymbol.GetNamespaceMembers())
        {
            yield return symbol;

            foreach (INamespaceSymbol? childNamespace in GetNamespaces(symbol))
            {
                yield return childNamespace;
            }
        }
    }
}