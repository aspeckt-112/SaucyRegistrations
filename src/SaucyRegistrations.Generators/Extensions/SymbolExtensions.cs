using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

public static class SymbolExtensions
{
	internal static AttributeData? GetFirstAttributeOfTypeOrNull<T>(this ISymbol symbol)
	{
		return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == typeof(T).Name);
	}
	
	internal static AttributeData? GetFirstAttributeWithNameOrNull(this ISymbol symbol, string attributeName)
	{
		return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == attributeName);
	}
	
	internal static AttributeData GetFirstAttributeWithName(this ISymbol symbol, string attributeName)
	{
		return symbol.GetAttributes().First(x => x.AttributeClass?.Name == attributeName);
	}
	
	internal static List<string> GetListOfStringsFromAttributeOnSymbol(
		this ISymbol symbol,
		string attributeName,
		string parameterName
	)
	{
		List<string> result = [];
		
		List<AttributeData> attributes = symbol.GetAttributesWithName(attributeName);

		if (attributes.Count == 0)
		{
			return result;
		}
		
		foreach (AttributeData attribute in attributes)
		{
			var value = attribute.GetParameter<string>(parameterName);

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
