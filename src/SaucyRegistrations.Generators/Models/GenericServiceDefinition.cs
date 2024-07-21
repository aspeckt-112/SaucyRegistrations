using System.Collections.Generic;

using SaucyRegistrations.Generators.Models.Contracts;

namespace SaucyRegistrations.Generators.Models;

/// <summary>
/// The generic service definition.
/// </summary>
/// <param name="fullQualifiedClassName">The fully qualified class name.</param>
/// <param name="serviceScope">The service scope.</param>
/// <param name="contractDefinitions">The contract definitions.</param>
/// <param name="key">The key.</param>
/// <param name="arity">The arity.</param>
public class GenericServiceDefinition(
    string fullQualifiedClassName,
    int? serviceScope,
    List<ContractDefinition>? contractDefinitions,
    string? key,
    int arity)
    : ServiceDefinition(fullQualifiedClassName, serviceScope, contractDefinitions, key)
{
    /// <summary>
    /// Gets the arity.
    /// </summary>
    public int Arity { get; } = arity;
}