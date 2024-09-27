using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamedTypeSymbol" /> class.
/// </summary>
internal static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Gets the fully qualified name of the named type symbol.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns>The fully qualified name of the named type symbol.</returns>
    internal static string GetFullyQualifiedName(this ITypeSymbol namedTypeSymbol)
    {
        var fullyQualifiedName = namedTypeSymbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrWhiteSpace(fullyQualifiedName))
        {
            fullyQualifiedName += ".";
        }

        fullyQualifiedName += namedTypeSymbol.Name;

        return fullyQualifiedName;
    }

    /// <summary>
    /// Checks if the named type symbol is one of the specified types.
    /// </summary>
    /// <param name="typeSymbol">The named type symbol.</param>
    /// <param name="types">The types to check.</param>
    /// <returns><c>true</c> if the named type symbol is one of the specified types; otherwise, <c>false</c>.</returns>
    internal static bool IsOneOf(this ITypeSymbol typeSymbol, params string[] types)
    {
        if (typeSymbol is null)
        {
            throw new ArgumentNullException(nameof(typeSymbol));
        }

        if (types is null)
        {
            throw new ArgumentNullException(nameof(types));
        }

        // Does the type symbol implement an interface in the types array?
        foreach (var type in types)
        {
            if (typeSymbol.AllInterfaces.Any(x => x.Name == type))
            {
                return true;
            }
        }

        // Does the type symbol have a base class?
        if (typeSymbol.BaseType is null)
        {
            return false;
        }

        // Does the base class implement an interface in the types array?
        foreach (var type in types)
        {
            if (typeSymbol.BaseType.AllInterfaces.Any(x => x.GetFullyQualifiedName() == type))
            {
                return true;
            }
        }

        return false;
    }
}