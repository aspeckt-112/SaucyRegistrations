using System.Linq;
using System.Text;

namespace SaucyRegistrations.Generators.Models.Contracts;

/// <summary>
/// The open generic contract definition.
/// </summary>
/// <param name="typeName">The type name.</param>
/// <param name="arity">The arity.</param>
public class OpenGenericContractDefinition(string typeName, int arity)
    : ContractDefinition(typeName)
{
    /// <summary>
    /// Gets the arity.
    /// </summary>
    public int Arity { get; } = arity;
}