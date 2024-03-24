using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models;

/// <summary>
/// Represents a service definition.
/// </summary>
public class ServiceDefinition(string fullQualifiedClassName, int? serviceScope, List<string>? contractNames)
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
    public List<string>? ContractNames { get; } = contractNames;

    /// <summary>
    /// Gets a value indicating whether the service definition has contracts.
    /// </summary>
    public bool HasContracts => ContractNames?.Count > 0;
}