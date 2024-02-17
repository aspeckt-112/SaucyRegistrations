using System;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="UseScope" /> attribute is used to specify the service scope for a class registration.
    /// If applied to a class, the class will be registered with the specified service scope, instead of the default service
    /// scope
    /// provided by the <see cref="DefaultScopeRegistration" /> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UseScope : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseScope" /> class.
        /// </summary>
        /// <param name="serviceScope">The service scope to use.</param>
        public UseScope(ServiceScope serviceScope)
        {
            ServiceScope = serviceScope;
        }

        /// <summary>
        /// Gets the service scope to use.
        /// </summary>
        public ServiceScope ServiceScope { get; }
    }
}