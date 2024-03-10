using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to mark a method as the target for Saucy registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SaucyRegistrationTarget : Attribute
    {
    }
}