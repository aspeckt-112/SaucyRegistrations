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
        var contactDefinitions = new List<ContractDefinition>();

        var hasAbstractBaseClass = namedTypeSymbol.BaseType is { IsAbstract: true };

        ImmutableHashSet<string> interfacesToExclude = GetDoNotRegisterInterfaceAttributes(namedTypeSymbol);
        ImmutableArray<INamedTypeSymbol> interfaces = GetFilteredInterfaces(namedTypeSymbol, interfacesToExclude);
        var interfaceCount = interfaces.Length;

        if (ShouldRegisterBaseClass(namedTypeSymbol, hasAbstractBaseClass))
        {
            contactDefinitions.Add(CreateContractDefinition(namedTypeSymbol.BaseType!));
            contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
        }
        else if (ShouldRegisterInterfaces(namedTypeSymbol, hasAbstractBaseClass, interfaceCount))
        {
            contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
        }
        else if (interfaceCount > 0)
        {
            contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
        }

        return contactDefinitions;
    }

    private static bool ShouldRegisterBaseClass(INamedTypeSymbol namedTypeSymbol, bool hasAbstractBaseClass)
    {
        var shouldRegisterAbstractClass = namedTypeSymbol.GetAttributes()
            .Any(x => x.AttributeClass?.Name == nameof(SaucyRegisterAbstractClass));
        return hasAbstractBaseClass && shouldRegisterAbstractClass;
    }

    private static bool ShouldRegisterInterfaces(
        INamedTypeSymbol namedTypeSymbol,
        bool hasAbstractBaseClass,
        int interfaceCount)
    {
        var shouldRegisterAbstractClass = namedTypeSymbol.GetAttributes()
            .Any(x => x.AttributeClass?.Name == nameof(SaucyRegisterAbstractClass));

        return hasAbstractBaseClass && interfaceCount > 0 && !shouldRegisterAbstractClass;
    }

    private static ImmutableHashSet<string> GetDoNotRegisterInterfaceAttributes(ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetAttributes()
            .Where(x => x.AttributeClass?.Name == nameof(SaucyDoNotRegisterWithInterface))
            .Select(x => x.ConstructorArguments[0].Value!.ToString())
            .ToImmutableHashSet();
    }

    private static ImmutableArray<INamedTypeSymbol> GetFilteredInterfaces(
        ITypeSymbol typeSymbol,
        ImmutableHashSet<string> interfacesToExclude)
    {
        return typeSymbol.Interfaces
            .Where(x => !interfacesToExclude.Contains(x.Name))
            .ToImmutableArray();
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