using Microsoft.CodeAnalysis;

using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Models;

/// <summary>
/// Represents a type that is to be registered with the service collection.
/// </summary>
internal class Type
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Type" /> class.
    /// </summary>
    /// <param name="symbol">The type that is to be registered.</param>
    /// <param name="serviceScope">The service scope of the type.</param>
    /// <returns>A new instance of the <see cref="Type" /> class.</returns>
    internal Type(ITypeSymbol symbol, ServiceScope serviceScope)
    {
        Symbol = symbol;
        ServiceScope = serviceScope;
    }

    /// <summary>
    /// Gets the type that is to be registered.
    /// </summary>
    public ITypeSymbol Symbol { get; private set; }

    /// <summary>
    /// Gets the service scope of the type.
    /// </summary>
    public ServiceScope ServiceScope { get; private set; }
}