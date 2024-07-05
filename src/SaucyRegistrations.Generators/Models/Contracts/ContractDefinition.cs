namespace SaucyRegistrations.Generators.Models.Contracts;

/// <summary>
/// Represents a contract definition.
/// </summary>
public class ContractDefinition(string typeName)
{
    /// <summary>
    /// Gets the type name of the contract.
    /// </summary>
    public string TypeName { get; } = typeName;
}