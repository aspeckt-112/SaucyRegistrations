using System;
using System.Diagnostics.CodeAnalysis;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to tell Saucy to include the class in the service collection.
    /// You can use this attribute if you don't want to include every class in a namespace.
    /// </summary>
    /// <usage>[SaucyAddType]<para />public class ExplicitlyRegistered{}</usage>
    [AttributeUsage(AttributeTargets.Class)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Not applicable")]
    public sealed class SaucyAddType : Attribute
    {
    }
}