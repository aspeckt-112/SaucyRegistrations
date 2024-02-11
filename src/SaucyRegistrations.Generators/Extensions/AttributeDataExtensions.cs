using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using SaucyRegistrations.Generators.Parameters;

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

    internal static T GetValueOfPropertyWithName<T>(this AttributeData attributeData, string propertyName)
    {
        return (T)attributeData.GetAttributeParameters().First(x => x.Name == propertyName).Value!;
    }

    internal static T GetValueForPropertyOfType<T>(this AttributeData attributeData, string parameterName)
    {
        return (T)attributeData.GetAttributeParameters().First(x => x.Name == parameterName).Value!;
    }

    private static List<AttributeParameter> GetAttributeParameters(this AttributeData attributeData)
    {
        ImmutableArray<IParameterSymbol> constructorParameters = attributeData.AttributeConstructor!.Parameters;

        ImmutableArray<TypedConstant> namedArguments = attributeData.ConstructorArguments;

        List<AttributeParameter> attributeParameters = new();

        for (int i = 0; i < namedArguments.Length; i++)
        {
            string parameterName = constructorParameters[i].Name.ToPascalCase();

            TypedConstant namedArgument = namedArguments[i];

            attributeParameters.Add(
                new AttributeParameter(
                    parameterName,
                    constructorParameters[i].Type,
                    namedArgument.Kind switch
                    {
                        TypedConstantKind.Array => namedArgument.Values,
                        _ => namedArgument.Value
                    }));
        }

        return attributeParameters;
    }
}