using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="IncludeSystemNamespaces" /> attribute is used to specify that the system namespaces should be included
    /// in the registration process.
    /// By default, the system namespaces are excluded from the registration process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IncludeSystemNamespaces : Attribute
    {
    }
}