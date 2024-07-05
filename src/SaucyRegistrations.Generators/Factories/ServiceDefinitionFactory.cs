using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.Models.Contracts;
using SaucyRegistrations.Generators.SourceConstants.Attributes;

namespace SaucyRegistrations.Generators.Factories;

/// <summary>
/// The factory for creating service definitions.
/// </summary>
internal static class ServiceDefinitionFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="ServiceDefinitionFactory"/> class.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <param name="serviceScope">The service scope.</param>
    /// <returns>The service definition.</returns>
    internal static ServiceDefinition CreateServiceDefinition(INamedTypeSymbol namedTypeSymbol, int serviceScope)
    {
        AttributeData? isKeyedServiceAttribute = namedTypeSymbol.GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == nameof(SaucyKeyedService));

        string? key = null;

        if (isKeyedServiceAttribute is not null)
        {
            key = isKeyedServiceAttribute.ConstructorArguments[0].Value?.ToString();
        }

        if (namedTypeSymbol.IsGenericType)
        {
            return new GenericServiceDefinition(
                namedTypeSymbol.GetFullyQualifiedName(),
                serviceScope,
                GetContractDefinitions(namedTypeSymbol),
                key,
                namedTypeSymbol.TypeArguments.Length);
        }

        return new ServiceDefinition(
            namedTypeSymbol.GetFullyQualifiedName(),
            serviceScope,
            GetContractDefinitions(namedTypeSymbol),
            key);
    }

    /// <summary>
    /// Gets the service definitions from the namespaces in the assembly.
    /// </summary>
    /// <param name="includeNamespaceSuffixAttributes">The include namespace suffix attributes.</param>
    /// <param name="namespacesInAssembly">The namespaces in the assembly.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The service definitions.</returns>
    internal static ImmutableArray<ServiceDefinition> GetServiceDefinitionsFromNamespaces(
        List<AttributeData> includeNamespaceSuffixAttributes,
        List<INamespaceSymbol> namespacesInAssembly,
        CancellationToken cancellationToken
    )
    {
        ImmutableArray<ServiceDefinition>.Builder immutableArrayBuilder =
            ImmutableArray.CreateBuilder<ServiceDefinition>();

        foreach (AttributeData? namespaceSuffixAttribute in includeNamespaceSuffixAttributes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var namespaceSuffix = namespaceSuffixAttribute.ConstructorArguments[0].Value?.ToString();
            var serviceScope = (int)namespaceSuffixAttribute.ConstructorArguments[1].Value!;

            if (namespaceSuffix is null)
            {
                continue;
            }

            var matchingNamespaces = namespacesInAssembly.Where(x => x.Name.EndsWith(namespaceSuffix)).ToList();

            AddServiceDefinitionsFromNamespaces(
                matchingNamespaces,
                serviceScope,
                immutableArrayBuilder,
                cancellationToken
            );
        }

        return immutableArrayBuilder.ToImmutable();
    }

    private static void AddServiceDefinitionsFromNamespaces(
        List<INamespaceSymbol> matchingNamespaces,
        int serviceScope,
        ImmutableArray<ServiceDefinition>.Builder immutableArrayBuilder,
        CancellationToken ct
    )
    {
        foreach (INamespaceSymbol? @namespace in matchingNamespaces)
        {
            ct.ThrowIfCancellationRequested();

            List<INamedTypeSymbol> instantiableTypesInNamespace = @namespace.GetInstantiableTypes();

            if (instantiableTypesInNamespace.Count == 0)
            {
                continue;
            }

            AddServiceDefinitionsFromTypes(instantiableTypesInNamespace, serviceScope, immutableArrayBuilder, ct);
        }
    }

    private static void AddServiceDefinitionsFromTypes(
        List<INamedTypeSymbol> instantiableTypesInNamespace,
        int serviceScope,
        ImmutableArray<ServiceDefinition>.Builder immutableArrayBuilder,
        CancellationToken ct
    )
    {
        foreach (INamedTypeSymbol? typeSymbol in instantiableTypesInNamespace)
        {
            ct.ThrowIfCancellationRequested();
            ServiceDefinition serviceDefinition = CreateServiceDefinition(typeSymbol, serviceScope);
            immutableArrayBuilder.Add(serviceDefinition);
        }
    }

    /// <summary>
    /// Gets the contracts for the named type symbol.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns>The contracts for the named type symbol.</returns>
    private static List<ContractDefinition> GetContractDefinitions(INamedTypeSymbol namedTypeSymbol)
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

        return
        [
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

    private static bool ShouldRegisterInterfaces(INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.Interfaces.Length > 0;
    }

    private static ContractDefinition CreateContractDefinition(INamedTypeSymbol typeSymbol)
    {
        var fullyQualifiedName = typeSymbol.GetFullyQualifiedName();

        if (typeSymbol.IsUnboundGenericType)
        {
            return new OpenGenericContractDefinition(fullyQualifiedName, typeSymbol.Arity);
        }

        if (!typeSymbol.IsGenericType)
        {
            return new ContractDefinition(fullyQualifiedName);
        }

        var allTypeArgumentsAreKnown = typeSymbol.TypeArguments.All(x => x is INamedTypeSymbol);

        if (!allTypeArgumentsAreKnown)
        {
            return new OpenGenericContractDefinition(fullyQualifiedName, typeSymbol.Arity);
        }

        var genericTypeNames = GetGenericTypeNames(typeSymbol);
        return new KnownNamedTypeSymbolGenericContractDefinition(fullyQualifiedName, genericTypeNames);
    }

    private static List<string> GetGenericTypeNames(INamedTypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsGenericType)
        {
            return [typeSymbol.GetFullyQualifiedName()];
        }

        var genericTypeNames = new List<string>();

        foreach (var argument in typeSymbol.TypeArguments)
        {
            if (argument is not INamedTypeSymbol namedTypeArgument)
            {
                continue;
            }

            var typeName = namedTypeArgument.GetFullyQualifiedName();
            if (namedTypeArgument.IsGenericType)
            {
                // Construct the name for nested generic types, including their namespaces
                var nestedGenericTypeNames = GetGenericTypeNames(namedTypeArgument);
                var nestedGenericTypeName = $"{typeName}<{string.Join(", ", nestedGenericTypeNames)}>";
                genericTypeNames.Add(nestedGenericTypeName);
            }
            else
            {
                // Directly add the fully qualified name for non-generic types
                genericTypeNames.Add(typeName);
            }
        }

        return genericTypeNames;
    }
}