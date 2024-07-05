using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Models;
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
                namedTypeSymbol.GetContractDefinitions(),
                key,
                namedTypeSymbol.TypeArguments.Length);
        }

        return new ServiceDefinition(
            namedTypeSymbol.GetFullyQualifiedName(),
            serviceScope,
            namedTypeSymbol.GetContractDefinitions(),
            key);
    }
}