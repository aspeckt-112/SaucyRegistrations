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
        List<ContractDefinition> contactDefinitions = new List<ContractDefinition>();

        var hasAbstractBaseClass = namedTypeSymbol.BaseType is { IsAbstract: true };
        var interfaces = namedTypeSymbol.Interfaces;
        var interfaceCount = interfaces.Length;

        var shouldRegisterAbstractClass = namedTypeSymbol.GetAttributes().Any(x => x.AttributeClass?.Name == nameof(SaucyRegisterAbstractClass));

        if (hasAbstractBaseClass && interfaceCount == 0)
        {
            // If the type only has an abstract base class, register it as a contract.
            contactDefinitions.Add(CreateContractDefinition(namedTypeSymbol.BaseType!));
        }
        else if (hasAbstractBaseClass && interfaceCount > 0)
        {
            if (!shouldRegisterAbstractClass)
            {
                // If the type has an abstract base class and interfaces, but does not have the SaucyRegisterAbstractClass attribute, register only the interfaces.
                contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
            }
            else
            {
                // If the type has an abstract base class and interfaces, and has the SaucyRegisterAbstractClass attribute, register the abstract base class and interfaces.
                contactDefinitions.Add(CreateContractDefinition(namedTypeSymbol.BaseType!));
                contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
            }
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