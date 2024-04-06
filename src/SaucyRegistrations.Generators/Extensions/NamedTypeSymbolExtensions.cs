using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.SourceConstants.Attributes;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamedTypeSymbol" /> class.
/// </summary>
internal static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Gets the fully qualified name of the named type symbol.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns>The fully qualified name of the named type symbol.</returns>
    internal static string GetFullyQualifiedName(this ITypeSymbol namedTypeSymbol)
    {
        var fullyQualifiedName = namedTypeSymbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrWhiteSpace(fullyQualifiedName))
        {
            fullyQualifiedName += ".";
        }

        fullyQualifiedName += namedTypeSymbol.Name;

        return fullyQualifiedName;
    }

    /// <summary>
    /// Gets the contracts for the named type symbol.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns>The contracts for the named type symbol.</returns>
    internal static List<ContractDefinition> GetContractDefinitions(this INamedTypeSymbol namedTypeSymbol)
    {
        List<ContractDefinition> contactDefinitions = [];

        foreach (INamedTypeSymbol? @interface in namedTypeSymbol.Interfaces)
        {
            contactDefinitions.Add(CreateContractDefinition(@interface));
        }

        // If the type symbol has the "SaucyOnlyRegisterInterface" attribute, the bail out early.
        if (namedTypeSymbol.GetAttributes().Any(x => x.AttributeClass?.Name == nameof(SaucyOnlyRegisterInterface)))
        {
            return contactDefinitions;
        }

        var hasAbstractBaseClass = namedTypeSymbol.BaseType is { IsAbstract: true };

        if (hasAbstractBaseClass && namedTypeSymbol.BaseType is not null)
        {
            contactDefinitions.Add(CreateContractDefinition(namedTypeSymbol.BaseType));
        }

        return contactDefinitions;
    }

    private static ContractDefinition CreateContractDefinition(INamedTypeSymbol typeSymbol)
    {
        List<string>? genericTypeNames = null;

        if (typeSymbol.IsGenericType)
        {
            genericTypeNames = typeSymbol.TypeArguments.Select(x => x.GetFullyQualifiedName()).ToList();
        }

        return new ContractDefinition(typeSymbol.GetFullyQualifiedName(), genericTypeNames);
    }
}