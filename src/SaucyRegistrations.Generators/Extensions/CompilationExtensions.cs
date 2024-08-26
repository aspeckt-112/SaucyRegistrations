using System;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Factories;
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
    internal static AssemblyAttributesProvider GetNamespacesToInclude(
        this IncrementalValueProvider<Compilation> provider)
    {
        return provider.Select(
            (c, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var includeNamespaceSuffixAttributes = c.Assembly.GetAttributes()
                    .Where(x => x.AttributeClass?.Name == nameof(SaucyIncludeNamespace)).ToList();

                if (includeNamespaceSuffixAttributes.Count == 0)
                {
                    return default;
                }

                var namespacesInAssembly = c.Assembly.GlobalNamespace.GetAllNestedNamespaces().ToList();

                return ServiceDefinitionFactory.GetServiceDefinitionsFromNamespaces(
                    includeNamespaceSuffixAttributes,
                    namespacesInAssembly,
                    ct);
            }
        );
    }
}