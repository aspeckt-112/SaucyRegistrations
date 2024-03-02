using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The extensions for the <see cref="INamespaceSymbol" /> type.
/// </summary>
internal static class NamespaceSymbolExtensions
{
    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> of <see cref="INamespaceSymbol" />.
    /// </summary>
    /// <remarks>Uses recursion to get all namespaces.</remarks>
    /// <param name="namespaceSymbol">The <see cref="INamespaceSymbol" /> to get the list of namespaces from.</param>
    /// <returns>A <see cref="IEnumerable{T}" /> of <see cref="INamespaceSymbol" />.</returns>
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
    /// Gets a <see cref="List{T}" /> of <see cref="INamedTypeSymbol" />. This will only return concrete types.
    /// </summary>
    /// <param name="namespaceSymbol">The <see cref="INamespaceSymbol" /> to get the list of types from.</param>
    /// <returns>A <see cref="List{T}" /> of <see cref="INamedTypeSymbol" />.</returns>
    internal static List<INamedTypeSymbol> GetInstantiableTypesInNamespace(this INamespaceSymbol namespaceSymbol)
    {
        List<INamedTypeSymbol> types = new();

        foreach (INamedTypeSymbol? symbol in namespaceSymbol.GetTypeMembers())
        {
            if (symbol.IsAbstract
                || symbol.IsStatic)
            {
                continue;
            }

            types.Add(symbol);
        }

        return types;
    }
}