using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The extensions for the <see cref="ISymbol" /> type.
/// </summary>
/// <seealso cref="ISymbol" />
internal static class SymbolExtensions
{
    /// <summary>
    /// Checks if the given symbol should be included in the source generation.
    /// The symbol should have the <see cref="IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute" /> attribute.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol" /> to check.</param>
    /// <returns><c>true</c> if the symbol should be included in the source generation; otherwise, <c>false</c>.</returns>
    internal static bool ShouldBeIncludedInSourceGeneration(this ISymbol symbol)
    {
        return symbol.HasAttributeOfType<IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute>();
    }

    private static bool HasAttributeOfType<T>(this ISymbol symbol)
    {
        return symbol.GetAttributes().Any(x => x.AttributeClass?.Name == typeof(T).Name);
    }

    /// <summary>
    /// Gets the first attribute with the given name or null if not found.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol" /> to get the attribute from.</param>
    /// <param name="attributeName">The name of the attribute to get.</param>
    /// <returns></returns>
    internal static AttributeData? GetFirstAttributeWithNameOrNull(this ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == attributeName);
    }

    internal static AttributeData? GetFirstAttributeOfTypeOrNull<T>(this ISymbol symbol)
    {
        return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == typeof(T).Name);
    }

    internal static AttributeData GetFirstAttributeWithName(this ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes().First(x => x.AttributeClass?.Name == attributeName);
    }

    /// <summary>
    /// Gets the value for the property of the given type from the attribute on the symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributeName"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    internal static List<string> GetListOfStringsFromAttributeOnSymbol(this ISymbol symbol, string attributeName, string parameterName)
    {
        List<string> result = [];

        List<AttributeData> attributes = symbol.GetAttributesWithName(attributeName);

        if (attributes.Count == 0)
        {
            return result;
        }

        foreach (AttributeData attribute in attributes)
        {
            var value = attribute.GetValueForPropertyOfType<string>(parameterName);

            if (!string.IsNullOrWhiteSpace(value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static List<AttributeData> GetAttributesWithName(this ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes().Where(x => x.AttributeClass?.Name == attributeName).ToList();
    }
}