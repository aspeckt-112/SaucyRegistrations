using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The extensions for the <see cref="ISymbol" /> type.
/// </summary>
/// <seealso cref="ISymbol" />
internal static class SymbolExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="ISymbol" /> should be included in source generation.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol" /> to check.</param>
    /// <param name="defaultServiceScope">The default service scope.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="ISymbol" /> should be included in source generation; otherwise,
    /// <c>false</c>.
    /// </returns>
    internal static bool ShouldBeIncludedInSourceGeneration(this ISymbol symbol, out ServiceScope? defaultServiceScope)
    {
        AttributeData? attribute = symbol.GetFirstAttributeOfType<DefaultScopeRegistration>();

        if (attribute is null)
        {
            defaultServiceScope = null;

            return false;
        }

        ServiceScope serviceScope = attribute.GetValueForPropertyOfType<ServiceScope>(nameof(DefaultScopeRegistration.DefaultServiceScope));

        defaultServiceScope = serviceScope;

        return true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="ISymbol" /> has an attribute of a given type.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol" /> to check.</param>
    /// <typeparam name="T">The type of the attribute to check for.</typeparam>
    /// <returns>
    /// <c>true</c> if the specified <see cref="ISymbol" /> has an attribute of the given type; otherwise,
    /// <c>false</c>.
    /// </returns>
    internal static bool HasAttributeOfType<T>(this ISymbol symbol)
    {
        return symbol.GetAttributes().Any(x => x.AttributeClass?.Name == typeof(T).Name);
    }

    /// <summary>
    /// Gets the first attribute with a given type.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol" /> to get the attribute from.</param>
    /// <typeparam name="T">The type of the attribute to get.</typeparam>
    /// <returns>The <see cref="AttributeData" /> of the attribute if found; otherwise, <c>null</c>.</returns>
    internal static AttributeData? GetFirstAttributeOfType<T>(this ISymbol symbol)
    {
        return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == typeof(T).Name);
    }

    /// <summary>
    /// Gets the attributes with a given type.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol" /> to get the attributes from.</param>
    /// <typeparam name="T">The type of the attribute to get.</typeparam>
    /// <returns>A <see cref="List{T}" /> of <see cref="AttributeData" />.</returns>
    internal static List<AttributeData> GetAttributesOfType<T>(this ISymbol symbol)
    {
        return symbol.GetAttributes().Where(x => x.AttributeClass?.Name == typeof(T).Name).ToList();
    }
}