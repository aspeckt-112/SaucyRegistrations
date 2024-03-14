using System;
using System.Text;

namespace SaucyRegistrations.Generators.Extensions
{
    internal static class StringBuilderExtensions
    {
        internal static StringBuilder AppendTwoNewLines(this StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine();

            return builder;
        }
    }
}