using System.Text;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The integer extensions.
/// </summary>
internal static class IntExtensions
{
    /// <summary>
    /// Gets the arity string.
    /// </summary>
    /// <param name="arity">The arity.</param>
    /// <returns>The arity string.</returns>
    internal static string GetArityString(this int arity)
    {
        if (arity == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append('<');

        for (var i = 1; i < arity; i++)
        {
            sb.Append(",");
        }

        sb.Append('>');
        return sb.ToString();
    }
}