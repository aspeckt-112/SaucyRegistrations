namespace SaucyRegistrations.Generators.Extensions;

public static class StringExtensions
{
	internal static string ToPascalCase(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}

		if (value.Length == 1)
		{
			return value.ToUpperInvariant();
		}

		return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
	}
}
