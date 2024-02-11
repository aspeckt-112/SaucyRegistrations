using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The WhenRegisteringWithContainerShouldExcludedAttribute is used to exclude classes from the source generation
    /// registration process.
    /// </summary>
    /// <remarks>If applied to a class, the class will be excluded from the registration process.</remarks>
    /// <usage>[WhenRegisteringWithContainerShouldExcluded]</usage>
    [AttributeUsage(AttributeTargets.Class)]

    // ReSharper disable once ClassNeverInstantiated.Global
    public class WhenRegisteringWithContainerShouldExcludedAttribute : Attribute
    {
    }
}