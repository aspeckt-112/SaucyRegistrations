namespace SaucyRegistrations.Generators.Models.Contracts;

/// <summary>
/// Represents a hosted service contract definition.
/// </summary>
/// <param name="typeName">The type name.</param>
public class HostedServiceContractDefinition(string typeName) : ContractDefinition(typeName)
{
}