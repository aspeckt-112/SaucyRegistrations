using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="IncludeMicrosoftNamespaces" /> attribute is used to specify that the Microsoft namespaces should be
    /// included in the registration process.
    /// By default, the Microsoft namespaces are excluded from the registration process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IncludeMicrosoftNamespaces : Attribute
    {
    }
}