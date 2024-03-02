using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extensions for the <see cref="AttributeData" /> type.
/// </summary>
public static class AttributeDataExtensions
{
    /// <summary>
    /// Determines whether the attribute is of the given type.
    /// </summary>
    /// <typeparam name="T">The type of the attribute to check for.</typeparam>
    /// <param name="attributeData">The attribute data to check.</param>
    /// <returns><c>true</c> if the attribute is of the given type; otherwise, <c>false</c>.</returns>
    internal static bool Is<T>(this AttributeData attributeData)
    {
        return attributeData.AttributeClass?.Name == typeof(T).Name;
    }

    /// <summary>
    /// Gets the value for the property of the given type from the attribute.
    /// </summary>
    /// <typeparam name="T">The type of the property to get the value for.</typeparam>
    /// <param name="attribute">The attribute to get the value from.</param>
    /// <param name="propertyName">The name of the property on the attribute to get the value for.</param>
    /// <returns>The value for the property of the given type from the attribute.</returns>
    internal static T GetPropertyValueAsType<T>(this AttributeData attribute, string propertyName)
    {
        return (T)attribute.ExtractAttributeParameters().First(x => x.Name == propertyName).Value!;
    }

    private static List<(string Name, object? Value)> ExtractAttributeParameters(this AttributeData attributeData)
    {
        List<(string Name, object? Value)> result = new();

        ImmutableArray<IParameterSymbol> constructorParameters = attributeData.AttributeConstructor!.Parameters;
        ImmutableArray<TypedConstant> namedArguments = attributeData.ConstructorArguments;

        for (var i = 0; i < namedArguments.Length; i++)
        {
            var parameterName = constructorParameters[i].Name.ToPascalCase();

            TypedConstant namedArgument = namedArguments[i];

            var value = namedArgument.Kind == TypedConstantKind.Array
                ? namedArgument.Values
                : namedArgument.Value;

            result.Add((parameterName, value));
        }

        return result;
    }
}