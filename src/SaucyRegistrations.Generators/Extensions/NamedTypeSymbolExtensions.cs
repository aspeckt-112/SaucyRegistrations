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
}