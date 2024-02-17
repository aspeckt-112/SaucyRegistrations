using System;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="DefaultScopeRegistration" /> attribute is used to specify the default service scope for registrations of
    /// classes within the assembly.
    /// If required, specific classes can be registered with a different service scope using the <see cref="UseScope" />
    /// attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DefaultScopeRegistration : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScopeRegistration" />
        /// class.
        /// </summary>
        /// <param name="defaultServiceScope">The service scope to use by default.</param>
        public DefaultScopeRegistration(ServiceScope defaultServiceScope)
        {
            DefaultServiceScope = defaultServiceScope;
        }

        /// <summary>
        /// Gets the service scope to use by default.
        /// </summary>
        public ServiceScope DefaultServiceScope { get; }
    }
}