using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Collections;
using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Logging;

using Type = SaucyRegistrations.Generators.Models.Type;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the <see cref="TypeSymbols"/>.
/// </summary>
internal class TypeSymbolsBuilder
{
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSymbolsBuilder"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="Logger"/>.</param>
    internal TypeSymbolsBuilder(Logger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Builds the <see cref="TypeSymbols"/>.
    /// </summary>
    /// <param name="assemblies">The <see cref="Assemblies"/> to build the <see cref="TypeSymbols"/> from.</param>
    /// <returns>An instance of the <see cref="TypeSymbols"/> class.</returns>
    internal TypeSymbols Build(Assemblies assemblies) => GetTypeSymbolsInAssemblies(assemblies);

    private TypeSymbols GetTypeSymbolsInAssemblies(Assemblies assemblies)
    {
        TypeSymbols result = new();

        _logger.WriteInformation("Building type symbols...");

        foreach (KeyValuePair<IAssemblySymbol, AssemblyScanConfiguration> assemblyMap in assemblies)
        {
            IAssemblySymbol? assembly = assemblyMap.Key;
            AssemblyScanConfiguration? scanConfiguration = assemblyMap.Value;

            _logger.WriteInformation($"Building type symbols for assembly: {assembly.Name}");

            Namespaces namespaces = GetNamespacesFromAssembly(assembly, scanConfiguration);

            if (namespaces.Count == 0)
            {
                _logger.WriteInformation($"No namespaces found in assembly: {assembly.Name}");
                continue;
            }

            _logger.WriteInformation($"Found {namespaces.Count} namespaces in assembly: {assembly.Name}");

            foreach (INamespaceSymbol @namespace in namespaces)
            {
                _logger.WriteInformation($"Building type symbols for namespace: {@namespace}");

                TypeSymbols types = GetTypesFromNamespace(@namespace, scanConfiguration!);

                foreach (Type? type in types)
                {
                    result.Add(type);
                }
            }
        }

        return result;
    }

    private Namespaces GetNamespacesFromAssembly(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyScanConfiguration)
    {
        Namespaces namespaces = new();

        _logger.WriteInformation($"Getting namespaces from assembly: {assemblySymbol.Name}");

        INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
        List<string> excludedNamespaces = assemblyScanConfiguration.ExcludedNamespaces;
        var includeMicrosoftNamespaces = assemblyScanConfiguration.IncludeMicrosoftNamespaces;
        var includeSystemNamespaces = assemblyScanConfiguration.IncludeSystemNamespaces;

        foreach (INamespaceSymbol @namespace in globalNamespace.GetNamespaces())
        {
            var namespaceName = @namespace.ToDisplayString();

            _logger.WriteInformation($"Checking if {namespaceName} should be included in source generation...");

            if (excludedNamespaces.Contains(namespaceName)
                || (namespaceName.StartsWith("Microsoft") && !includeMicrosoftNamespaces)
                || (namespaceName.StartsWith("System") && !includeSystemNamespaces))
            {
                _logger.WriteInformation($"{namespaceName} should be excluded from source generation.");
                continue;
            }

            namespaces.Add(@namespace);
        }

        return namespaces;
    }

    private TypeSymbols GetTypesFromNamespace(INamespaceSymbol @namespace, AssemblyScanConfiguration assemblyScanConfiguration)
    {
        TypeSymbols result = new();

        _logger.WriteInformation($"Getting concrete types from namespace: {@namespace}");

        List<INamedTypeSymbol> concreteTypes = @namespace.GetConcreteTypes();

        if (concreteTypes.Count == 0)
        {
            _logger.WriteInformation($"No concrete types found in namespace: {@namespace}");
            return result;
        }

        var assemblyHasOneOrMoreClassSuffix = assemblyScanConfiguration.ClassSuffixes.Count > 0;

        _logger.WriteInformation($"Found {concreteTypes.Count} concrete types in namespace: {@namespace}");

        foreach (INamedTypeSymbol typeSymbol in concreteTypes)
        {
            ServiceScope? typeServiceScope = null;

            _logger.WriteInformation($"Checking if {typeSymbol.Name} has a service scope attribute...");

            AttributeData? serviceScopeAttribute = typeSymbol.GetFirstAttributeOfType<UseScope>();

            if (serviceScopeAttribute is not null)
            {
                _logger.WriteInformation($"{typeSymbol.Name} has a service scope attribute.");
                typeServiceScope = serviceScopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(UseScope.ServiceScope));
            }
            else
            {
                _logger.WriteInformation($"{typeSymbol.Name} does not have a service scope attribute. Using the default service scope from the assembly scan configuration.");
            }

            ServiceScope serviceScope = typeServiceScope ?? assemblyScanConfiguration.DefaultServiceScope;

            // Check to see if the type is being explicitly included.
            if (typeSymbol.HasAttributeOfType<IncludeRegistration>())
            {
                _logger.WriteInformation($"{typeSymbol.Name} has an include registration attribute.");
                result.Add(new Type(typeSymbol, serviceScope));
                continue;
            }

            // If it's not explicitly included, check to see if it ends with any of the class suffixes.
            if (assemblyHasOneOrMoreClassSuffix)
            {
                _logger.WriteInformation($"Checking if {typeSymbol.Name} should be included in source generation...");

                if (typeSymbol.HasAttributeOfType<ExcludeRegistration>())
                {
                    _logger.WriteInformation($"{typeSymbol.Name} should be excluded from source generation.");
                    continue;
                }

                _logger.WriteInformation($"Checking if {typeSymbol.Name} ends with any of the class suffixes...");

                foreach (var suffix in assemblyScanConfiguration.ClassSuffixes)
                {
                    _logger.WriteInformation($"Checking if {typeSymbol.Name} ends with {suffix}...");

                    if (typeSymbol.Name.EndsWith(suffix))
                    {
                        _logger.WriteInformation($"{typeSymbol.Name} ends with {suffix}. Adding to type symbols with service scope: {serviceScope}");
                        result.Add(new Type(typeSymbol, serviceScope));
                    }
                }
            }
        }

        return result;
    }
}