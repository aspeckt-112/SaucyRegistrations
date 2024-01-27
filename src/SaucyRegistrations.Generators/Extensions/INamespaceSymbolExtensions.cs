using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

internal static class NamespaceSymbolExtensions
{
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
