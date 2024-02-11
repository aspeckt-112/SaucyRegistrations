using System;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute is used to include an assembly in the source
    /// generation registration process.
    /// </summary>
    /// <remarks>Apply this attribute to an assembly to include the assembly in the source generation registration process.</remarks>
    /// <usage>[assembly: IncludeInSourceGenerationRegistrationWithDefaultServiceScope(ServiceScope.Transient)]</usage>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute" />
        /// class.
        /// </summary>
        /// <param name="defaultServiceScope">The service scope to use by default.</param>
        public IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute(ServiceScope defaultServiceScope)
        {
            DefaultServiceScope = defaultServiceScope;
        }

        /// <summary>
        /// Gets the service scope to use by default.
        /// </summary>
        public ServiceScope DefaultServiceScope { get; }
    }
}