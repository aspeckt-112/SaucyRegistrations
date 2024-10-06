using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

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
    /// <returns>A value tuple of the namespace and the service scope.</returns>
    internal static List<(INamespaceSymbol Namespace, int ServiceScope)> NamespacesWithSaucyIncludeAttribute(this IAssemblySymbol assemblySymbol)
    {
        ImmutableArray<AttributeData> attributes = assemblySymbol.GetAttributes();

        var result = new List<(INamespaceSymbol Namespace, int ServiceScope)>();

        foreach (AttributeData? attribute in attributes)
        {
            if (attribute?.AttributeClass is { Name: nameof(SaucyIncludeNamespace) })
            {
                if (attribute.ConstructorArguments.Length < 2)
                {
                    continue;
                }

                if (attribute.ConstructorArguments[0].Value is not string userDefinedNamespace)
                {
                    continue;
                }

                if (attribute.ConstructorArguments[1].Value is not int serviceScope)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(userDefinedNamespace) || serviceScope < 0)
                {
                    continue;
                }

                var descendantNamespaces = assemblySymbol.GlobalNamespace.GetDescendantNamespaces();

                INamespaceSymbol? namespaceSymbol = descendantNamespaces.FirstOrDefault(ns => ns.Name == userDefinedNamespace || ns.Name.EndsWith($".{userDefinedNamespace}"));

                if (namespaceSymbol is null)
                {
                    continue;
                }

                result.Add((namespaceSymbol, serviceScope));
            }
        }

        return result;
    }
}