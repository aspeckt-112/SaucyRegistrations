using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="IncludeRegistration" /> attribute is used to specify the class to include in the registration process.
    /// Use this attribute to include classes that are not automatically registered with a class suffix.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeRegistration : Attribute
    {
    }
}