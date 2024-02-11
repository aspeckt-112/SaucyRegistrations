using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The WhenRegisteringShouldIncludeSystemNamespacesAttribute is used to include System namespaces in the source generation
    /// registration process.
    /// </summary>
    /// <remarks>Apply this attribute to an assembly to include System classes in the registration process.</remarks>
    /// <usage>[assembly: WhenRegisteringShouldIncludeSystemNamespacesAttribute]</usage>
    [AttributeUsage(AttributeTargets.Assembly)]

    // ReSharper disable once ClassNeverInstantiated.Global
    public class WhenRegisteringShouldIncludeSystemNamespacesAttribute : Attribute
    {
    }
}