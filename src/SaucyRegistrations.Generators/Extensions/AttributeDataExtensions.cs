using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using SaucyRegistrations.Generators.Parameters;

namespace SaucyRegistrations.Generators.Extensions;

public static class AttributeDataExtensions
{
	internal static bool Is<T>(this AttributeData attributeData) => attributeData.AttributeClass?.Name == typeof(T).Name;
	
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

		for (var i = 0; i < namedArguments.Length; i++)
		{
			string parameterName = constructorParameters[i].Name.ToPascalCase();

			TypedConstant namedArgument = namedArguments[i];

			attributeParameters.Add(
				new AttributeParameter(
					parameterName, constructorParameters[i].Type, namedArgument.Kind switch
					{
						TypedConstantKind.Array => namedArgument.Values,
						_ => namedArgument.Value
					}
				)
			);
		}

		return attributeParameters;
	}
}
