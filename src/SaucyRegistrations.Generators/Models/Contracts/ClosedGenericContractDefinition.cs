using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models.Contracts;

/// <summary>
/// The closed generic contract definition.
/// </summary>
public class ClosedGenericContractDefinition(string typeName, IEnumerable<string> genericTypeNames)
    : ContractDefinition(typeName)
{
    /// <summary>
    /// Gets the generic type names.
    /// </summary>
    public IEnumerable<string> GenericTypeNames { get; } = genericTypeNames;
}