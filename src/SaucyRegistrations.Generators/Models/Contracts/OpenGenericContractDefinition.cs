using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models.Contracts;

/// <summary>
/// The open generic contract definition.
/// </summary>
public class OpenGenericContractDefinition(string typeName, int arity)
    : ContractDefinition(typeName)
{
    /// <summary>
    /// Gets the arity of the closed generic contract.
    /// </summary>
    public int Arity { get; } = arity;
}