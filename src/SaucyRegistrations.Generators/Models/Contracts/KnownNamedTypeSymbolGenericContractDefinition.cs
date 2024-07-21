using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models.Contracts;

/// <summary>
/// The known named type symbol generic contract definition.
/// </summary>
/// <param name="typeName">The type name.</param>
/// <param name="genericTypeNames">The generic type names.</param>
public class KnownNamedTypeSymbolGenericContractDefinition(string typeName, IEnumerable<string> genericTypeNames)
    : ContractDefinition(typeName)
{
    /// <summary>
    /// Gets the generic type names.
    /// </summary>
    public IEnumerable<string> GenericTypeNames { get; } = genericTypeNames;
}