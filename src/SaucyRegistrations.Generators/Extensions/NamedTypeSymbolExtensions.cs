using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.Models.Contracts;
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

        ImmutableArray<INamedTypeSymbol> interfaces = GetFilteredInterfaces(namedTypeSymbol);

        if (ShouldRegisterBaseClass(namedTypeSymbol))
        {
            contactDefinitions.Add(CreateContractDefinition(namedTypeSymbol.BaseType!));
            contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
        }
        else if (ShouldRegisterInterfaces(namedTypeSymbol))
        {
            contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
        }
        else if (interfaces.Length > 0)
        {
            contactDefinitions.AddRange(interfaces.Select(CreateContractDefinition));
        }

        return contactDefinitions;
    }

    private static ImmutableArray<INamedTypeSymbol> GetFilteredInterfaces(ITypeSymbol typeSymbol)
    {
        var interfacesToExclude = typeSymbol.GetAttributes()
            .Where(x => x.AttributeClass?.Name == nameof(SaucyDoNotRegisterWithInterface))
            .Select(x => x.ConstructorArguments[0].Value!.ToString())
            .ToImmutableHashSet();

        return [
            ..typeSymbol.Interfaces
                .Where(x => !interfacesToExclude.Contains(x.Name))
        ];
    }

    private static bool ShouldRegisterBaseClass(INamedTypeSymbol namedTypeSymbol)
    {
        var hasAbstractBaseClass = namedTypeSymbol.BaseType is { IsAbstract: true };

        var shouldRegisterAbstractClass = namedTypeSymbol.GetAttributes()
            .Any(x => x.AttributeClass?.Name == nameof(SaucyRegisterAbstractClass));

        return hasAbstractBaseClass && shouldRegisterAbstractClass;
    }

    private static bool ShouldRegisterInterfaces(INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.Interfaces.Length > 0;

    private static ContractDefinition CreateContractDefinition(INamedTypeSymbol typeSymbol)
    {
        var fullyQualifiedName = typeSymbol.GetFullyQualifiedName();

        if (typeSymbol.IsUnboundGenericType)
        {
            return new OpenGenericContractDefinition(fullyQualifiedName, typeSymbol.Arity);
        }

        if (typeSymbol.IsGenericType)
        {
            var allTypeArgumentsAreKnown = typeSymbol.TypeArguments.All(x => x is INamedTypeSymbol);

            if (allTypeArgumentsAreKnown)
            {
                var genericTypeNames = typeSymbol.TypeArguments.Select(x => x.GetFullyQualifiedName()).ToList();
                return new KnownNamedTypeSymbolGenericContractDefinition(fullyQualifiedName, genericTypeNames);
            }

            return new OpenGenericContractDefinition(fullyQualifiedName, typeSymbol.Arity);
        }

        return new ContractDefinition(fullyQualifiedName);
    }
}