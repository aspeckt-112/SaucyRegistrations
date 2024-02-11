using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The WhenRegisteringShouldIncludeMicrosoftNamespacesAttribute is used to include Microsoft namespaces in the source generation registration process.
    /// </summary>
    /// <remarks>Apply this attribute to an assembly to include Microsoft classes in the registration process.</remarks>
    /// <usage>[assembly: WhenRegisteringShouldIncludeMicrosoftNamespacesAttribute]</usage>
    [AttributeUsage(AttributeTargets.Assembly)]

    // ReSharper disable once ClassNeverInstantiated.Global
    public class WhenRegisteringShouldIncludeMicrosoftNamespacesAttribute : Attribute
    {
    }
}