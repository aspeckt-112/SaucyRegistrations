using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Collections;
using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;

using Type = SaucyRegistrations.Generators.Models.Type;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// A builder for the <see cref="RunConfiguration" /> class.
/// </summary>
internal static class RunConfigurationBuilder
{
    /// <summary>
    /// Builds a <see cref="RunConfiguration" /> object from the provided <see cref="GenerationConfiguration" /> and <see cref="Assemblies" />.
    /// </summary>
    /// <param name="generationConfiguration">The <see cref="GenerationConfiguration"/>.</param>
    /// <param name="assemblies">The <see cref="Assemblies"/>.</param>
    /// <returns>A <see cref="RunConfiguration" /> object.</returns>
    internal static RunConfiguration Build(GenerationConfiguration generationConfiguration, Assemblies assemblies)
    {
        TypeSymbols allTypesInAllAssemblies = GetAllTypeSymbolsInAllAssemblies(assemblies);

        return new RunConfiguration(generationConfiguration, allTypesInAllAssemblies);
    }

    private static TypeSymbols GetAllTypeSymbolsInAllAssemblies(Assemblies assemblies)
    {
        TypeSymbols result = new();

        foreach (KeyValuePair<IAssemblySymbol, AssemblyScanConfiguration> assemblyMap in assemblies)
        {
            IAssemblySymbol? assembly = assemblyMap.Key;
            AssemblyScanConfiguration? scanConfiguration = assemblyMap.Value;

            Namespaces namespaces = GetNamespacesFromAssembly(assembly, scanConfiguration);

            if (namespaces.Count == 0)
            {
                continue;
            }

            foreach (INamespaceSymbol? @namespace in namespaces)
            {
                TypeSymbols types = GetTypesFromNamespace(@namespace!, scanConfiguration!);

                foreach (Type? type in types)
                {
                    result.Add(type);
                }
            }
        }

        return result;
    }

    private static Namespaces GetNamespacesFromAssembly(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyScanConfiguration)
    {
        Namespaces namespaces = new();

        INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
        List<string> excludedNamespaces = assemblyScanConfiguration.ExcludedNamespaces;
        var includeMicrosoftNamespaces = assemblyScanConfiguration.IncludeMicrosoftNamespaces;
        var includeSystemNamespaces = assemblyScanConfiguration.IncludeSystemNamespaces;

        foreach (INamespaceSymbol @namespace in globalNamespace.GetNamespaces())
        {
            var namespaceName = @namespace.ToDisplayString();

            if (excludedNamespaces.Contains(namespaceName)
                || (namespaceName.StartsWith("Microsoft") && !includeMicrosoftNamespaces)
                || (namespaceName.StartsWith("System") && !includeSystemNamespaces))
            {
                continue;
            }

            namespaces.Add(@namespace);
        }

        return namespaces;
    }

    private static TypeSymbols GetTypesFromNamespace(INamespaceSymbol @namespace, AssemblyScanConfiguration assemblyScanConfiguration)
    {
        TypeSymbols result = new();

        List<INamedTypeSymbol> concreteTypes = @namespace.GetConcreteTypes();

        if (concreteTypes.Count == 0)
        {
            return result;
        }

        var assemblyHasOneOrMoreClassSuffix = assemblyScanConfiguration.ClassSuffixes.Count > 0;

        foreach (INamedTypeSymbol typeSymbol in concreteTypes)
        {
            if (typeSymbol.HasAttributeOfType<ExcludeRegistration>())
            {
                continue;
            }

            ServiceScope? typeServiceScope = null;

            AttributeData? serviceScopeAttribute = typeSymbol.GetFirstAttributeOfType<UseScope>();

            if (serviceScopeAttribute is not null)
            {
                typeServiceScope = serviceScopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(UseScope.ServiceScope));
            }

            ServiceScope serviceScope = typeServiceScope ?? assemblyScanConfiguration.DefaultServiceScope;

            if (assemblyHasOneOrMoreClassSuffix)
            {
                foreach (var suffix in assemblyScanConfiguration.ClassSuffixes)
                {
                    if (typeSymbol.Name.EndsWith(suffix))
                    {
                        result.Add(new Type(typeSymbol, serviceScope));
                    }
                }

                continue;
            }

            result.Add(new Type(typeSymbol, serviceScope));
        }

        return result;
    }
}