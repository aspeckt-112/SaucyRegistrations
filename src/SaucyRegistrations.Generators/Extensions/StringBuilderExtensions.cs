using System.Text;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="StringBuilder" /> class.
/// </summary>
internal static class StringBuilderExtensions
{
    /// <summary>
    /// Appends two new lines to the string builder.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <returns>The string builder with two new lines appended.</returns>
    internal static StringBuilder AppendTwoNewLines(this StringBuilder builder)
    {
        builder.AppendLine();
        builder.AppendLine();

        return builder;
    }
}