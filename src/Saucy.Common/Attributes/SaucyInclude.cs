using System;
using System.Diagnostics.CodeAnalysis;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to tell Saucy to include the assembly in the service collection.
    /// You must apply this attribute to at least one assembly in your project to use Saucy.
    /// </summary>
    /// <usage>[assembly: SaucyInclude]</usage>
    [AttributeUsage(AttributeTargets.Assembly)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Not applicable")]
    public sealed class SaucyInclude : Attribute
    {
    }
}