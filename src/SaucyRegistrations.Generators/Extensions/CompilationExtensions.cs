using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.CodeConstants;
using SaucyRegistrations.Generators.Models;

namespace SaucyRegistrations.Generators.Extensions
{
    internal static class CompilationExtensions
    {
        internal static IncrementalValueProvider<string> GetAssemblyName(this IncrementalValueProvider<Compilation> compilation)
        {
            return compilation.Select(
                (c, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    return c.AssemblyName?.Replace(".", string.Empty) ?? string.Empty;
                }
            );
        }

        internal static IncrementalValueProvider<ImmutableArray<ServiceDefinition>> GetNamespacesToInclude(this IncrementalValueProvider<Compilation> provider)
        {
            return provider.Select(
                (c, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    var includeNamespaceSuffixAttributes
                        = c.Assembly.GetAttributes().Where(x => x.AttributeClass?.Name == AttributeConstants.SaucyIncludeNamespaceWithSuffixAttributeName).ToList();

                    if (includeNamespaceSuffixAttributes.Count == 0)
                    {
                        return default;
                    }

                    ImmutableArray<ServiceDefinition>.Builder immutableArrayBuilder = ImmutableArray.CreateBuilder<ServiceDefinition>();

                    var namespacesInAssembly = c.Assembly.GlobalNamespace.GetAllNestedNamespaces().ToList();

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

                        foreach (INamespaceSymbol? @namespace in matchingNamespaces)
                        {
                            ct.ThrowIfCancellationRequested();

                            List<INamedTypeSymbol> instantiableTypesInNamespace = @namespace.GetInstantiableTypes();

                            if (instantiableTypesInNamespace.Count == 0)
                            {
                                continue;
                            }

                            foreach (var typeSymbol in instantiableTypesInNamespace)
                            {
                                ct.ThrowIfCancellationRequested();

                                var contractNames = typeSymbol.GetContracts();

                                var serviceDefinition = new ServiceDefinition(typeSymbol.GetFullyQualifiedName(), serviceScope, contractNames);

                                immutableArrayBuilder.Add(serviceDefinition);
                            }
                        }
                    }

                    return immutableArrayBuilder.ToImmutable();
                }
            );
        }
    }
}