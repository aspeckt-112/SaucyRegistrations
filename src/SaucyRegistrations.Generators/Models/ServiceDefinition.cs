using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models;

/// <summary>
/// Represents a service definition.
/// </summary>
public class ServiceDefinition(string fullQualifiedClassName, int? serviceScope, List<ContractDefinition>? contractDefinitions, string? key)
{
    /// <summary>
    /// Gets the fully qualified class name.
    /// </summary>
    public string FullyQualifiedClassName { get; } = fullQualifiedClassName;

    /// <summary>
    /// Gets the service scope.
    /// </summary>
    public int? ServiceScope { get; } = serviceScope;

    /// <summary>
    /// Gets the contract names.
    /// </summary>
    public List<ContractDefinition>? ContractDefinitions { get; } = contractDefinitions;

    /// <summary>
    /// Gets a value indicating whether the service definition has contracts.
    /// </summary>
    public bool HasContracts => ContractDefinitions?.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the service definition is keyed.
    /// </summary>
    public string? Key { get; } = key;

    /// <summary>
    /// Gets a value indicating whether the service definition is keyed.
    /// </summary>
    public bool IsKeyed => !string.IsNullOrWhiteSpace(Key);
}