using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Factories;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Compilation" /> class.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Gets the assembly name from the compliation provider.
    /// </summary>
    /// <param name="compliationProvider">The compliation provider.</param>
    /// <returns>The assembly name.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    internal static AssemblyName GetAssemblyName(this CompilationProvider compliationProvider)
    {
        return compliationProvider
            .Select((compliation, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                return compliation.AssemblyName ?? string.Empty;
            });
    }

    /// <summary>
    /// Gets the namespaces to include in the registration process from the compilation provider.
    /// </summary>
    /// <param name="compilationProvider">The compilation provider.</param>
    /// <returns>Service definitions from the included namespaces.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    internal static ServiceDefinitionsFromNamespace GetServiceDefinitionsFromIncludedNamespaces(this CompilationProvider compilationProvider)
    {
        return compilationProvider
            .Select((compilation, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                List<(INamespaceSymbol Namespace, int ServiceScope)> includedNamespacesWithScopes = compilation
                    .Assembly
                    .NamespacesWithSaucyIncludeAttribute();

                if (includedNamespacesWithScopes.Count == 0)
                {
                    return default;
                }

                return ServiceDefinitionFactory.GetServiceDefinitionsFromNamespaces(includedNamespacesWithScopes, ct);
            });
    }
}