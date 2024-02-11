using System;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The WhenRegisteringUseScopeAttribute is used to specify the service scope for a class during the source generation registration
    /// process.
    /// </summary>
    /// <remarks>
    /// If applied to a class, the <see cref="ServiceScope" /> applied to the
    /// <see cref="IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute" /> will be ignored and the service
    /// scope provided to this attribute will be used instead.
    /// </remarks>
    /// <usage>[WhenRegisteringUseScope(ServiceScope.SERVICE_SCOPE_HERE)]</usage>
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenRegisteringUseScopeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhenRegisteringUseScopeAttribute" /> class.
        /// </summary>
        /// <param name="serviceScope">The service scope to use.</param>
        public WhenRegisteringUseScopeAttribute(ServiceScope serviceScope)
        {
            ServiceScope = serviceScope;
        }

        /// <summary>
        /// Gets the service scope to use.
        /// </summary>
        public ServiceScope ServiceScope { get; }
    }
}