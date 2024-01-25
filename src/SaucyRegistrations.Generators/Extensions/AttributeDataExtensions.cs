using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

public static class AttributeDataExtensions
{
	internal static T GetParameter<T>(this AttributeData attributeData, string parameterName)
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
