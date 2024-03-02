using System;
using System.Diagnostics.CodeAnalysis;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to tell Saucy to exclude the class from the service collection.
    /// You can use this attribute if you've used SaucyAddNamespace and want to exclude a specific class.
    /// </summary>
    /// <usage>[SaucyExclude]<para />public class ExplicitlyExcluded{}</usage>
    [AttributeUsage(AttributeTargets.Class)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Not applicable")]
    public sealed class SaucyExclude : Attribute
    {
    }
}