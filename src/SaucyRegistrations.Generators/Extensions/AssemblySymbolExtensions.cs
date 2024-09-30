using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.SourceConstants.Attributes;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="IAssemblySymbol" /> class.
/// </summary>
internal static class AssemblySymbolExtensions
{
    /// <summary>
    /// From the given assembly, get all the attributes that are of type <see cref="SaucyIncludeNamespace"/>.
    /// </summary>
    /// <param name="assemblySymbol">The assembly symbol.</param>
    /// <returns>The attributes that are of type <see cref="SaucyIncludeNamespace"/>.</returns>
    internal static SaucyIncludeNamespaceAttributes SaucyIncludeNamespaceAttributes(this IAssemblySymbol assemblySymbol)
    {
        var result = new SaucyIncludeNamespaceAttributes();

        foreach (AttributeData? attribute in assemblySymbol.GetAttributes())
        {
            if (attribute?.AttributeClass is { Name: nameof(SaucyIncludeNamespace) })
            {
                result.Add(attribute);
            }
        }

        return result;
    }
}