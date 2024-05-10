using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models;

/// <summary>
/// Represents a contract definition.
/// </summary>
public class ContractDefinition(string fullyQualifiedTypeName, List<string>? fullyQualifiedGenericTypeNames = null)
{
    /// <summary>
    /// Gets the fullyQualifiedTypeName of the contract.
    /// </summary>
    public string FullyQualifiedTypeName { get; } = fullyQualifiedTypeName;

    /// <summary>
    /// Gets the generic type of the contract.
    /// </summary>
    public List<string>? FullyQualifiedGenericTypeNames { get; } = fullyQualifiedGenericTypeNames;

    /// <summary>
    /// Gets a value indicating whether the contract is a generic type.
    /// </summary>
    public bool IsGeneric => FullyQualifiedGenericTypeNames is not null;
}