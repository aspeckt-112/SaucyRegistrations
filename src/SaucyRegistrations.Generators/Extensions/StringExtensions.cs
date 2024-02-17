namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// General string extensions.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Converts the string to PascalCase.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The value in PascalCase.</returns>
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