namespace SaucyRegistrations.Generators.Helpers;

/// <summary>
/// A class that contains helper methods for strings.
/// </summary>
internal static class StringHelpers
{
    /// <summary>
    /// Normalizes the input string by replacing all occurrences of "\r\n" with "\n".
    /// </summary>
    /// <param name="input">The input string to normalize.</param>
    /// <returns>The normalized string.</returns>
    internal static string Normalize(string input)
    {
        return input.Replace("\r\n", "\n");
    }
}