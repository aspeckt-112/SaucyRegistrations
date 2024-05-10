using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.SourceConstants.Attributes;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Compilation" /> class.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Gets the assembly name from the compilation.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The assembly name.</returns>
    internal static AssemblyNameProvider GetAssemblyName(this IncrementalValueProvider<Compilation> compilation)
    {
        return compilation.Select(
            (c, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return c.AssemblyName ?? string.Empty;
            }
        );
    }

    /// <summary>
    /// Gets the namespaces to include in the registration process.
    /// </summary>
    /// <param name="provider">The compilation provider.</param>
    /// <returns>The namespaces to include in the registration process.</returns>
    /// <remarks>
    /// This method will return an empty collection if no namespaces are found to include.
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    internal static AssemblyAttributesProvider GetNamespacesToInclude(this IncrementalValueProvider<Compilation> provider)
    {
        return provider.Select(
            (c, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                List<AttributeData> includeNamespaceSuffixAttributes = c.Assembly.GetAttributes()
                    .Where(x => x.AttributeClass?.Name == nameof(SaucyIncludeNamespaceWithSuffix)).ToList();

                if (includeNamespaceSuffixAttributes.Count == 0)
                {
                    return default;
                }

                var namespacesInAssembly = c.Assembly.GlobalNamespace.GetAllNestedNamespaces().ToList();

                return GetServiceDefinitions(includeNamespaceSuffixAttributes, namespacesInAssembly, ct);
            }
        );
    }

    private static ImmutableArray<ServiceDefinition> GetServiceDefinitions(
        List<AttributeData> includeNamespaceSuffixAttributes,
        List<INamespaceSymbol> namespacesInAssembly,
        CancellationToken ct
    )
    {
        ImmutableArray<ServiceDefinition>.Builder immutableArrayBuilder = ImmutableArray.CreateBuilder<ServiceDefinition>();

        foreach (AttributeData? namespaceSuffixAttribute in includeNamespaceSuffixAttributes)
        {
            ct.ThrowIfCancellationRequested();

            var namespaceSuffix = namespaceSuffixAttribute.ConstructorArguments[0].Value?.ToString();
            var serviceScope = (int)namespaceSuffixAttribute.ConstructorArguments[1].Value!;

            if (namespaceSuffix is null)
            {
                continue;
            }

            var matchingNamespaces = namespacesInAssembly.Where(x => x.Name.EndsWith(namespaceSuffix)).ToList();

            AddServiceDefinitionsFromNamespaces(matchingNamespaces, serviceScope, immutableArrayBuilder, ct);
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
            var serviceDefinition = new ServiceDefinition(typeSymbol.GetFullyQualifiedName(), serviceScope, typeSymbol.GetContractDefinitions());
            immutableArrayBuilder.Add(serviceDefinition);
        }
    }
}