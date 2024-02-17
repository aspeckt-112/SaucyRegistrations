using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="ExcludeRegistration" /> attribute is used to specify the class to exclude from the registration process.
    /// If applied, the class will not be registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeRegistration : Attribute
    {
    }
}