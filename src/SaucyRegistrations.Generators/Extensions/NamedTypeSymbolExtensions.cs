using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

internal static class NamedTypeSymbolExtensions
{
    internal static string GetFullyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
    {
        var fullyQualifiedName = namedTypeSymbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrWhiteSpace(fullyQualifiedName))
        {
            fullyQualifiedName += ".";
        }

        fullyQualifiedName += namedTypeSymbol.Name;

        return fullyQualifiedName;
    }

    internal static List<string> GetContracts(this INamedTypeSymbol namedTypeSymbol)
    {
        var contractNames = new List<string>();

        foreach (var interfaceType in namedTypeSymbol.Interfaces)
        {
            contractNames.Add(interfaceType.GetFullyQualifiedName());
        }

        var hasAbstractBaseClass = namedTypeSymbol.BaseType != null && namedTypeSymbol.BaseType.IsAbstract;

        if (hasAbstractBaseClass)
        {
            contractNames.Add(namedTypeSymbol.BaseType.GetFullyQualifiedName());
        }

        return contractNames;
    }
}