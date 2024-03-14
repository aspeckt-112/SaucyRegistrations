using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions
{
    internal static class NamespaceSymbolExtensions
    {
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

        internal static List<INamedTypeSymbol> GetInstantiableTypes(this INamespaceSymbol namespaceSymbol)
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
}