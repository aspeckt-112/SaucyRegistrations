using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The WhenRegisteringExcludeClassesInNamespaceAttribute is used to exclude classes in a specific namespace from being
    /// registered.
    /// </summary>
    /// <remarks>Apply this attribute to an assembly to exclude classes in the specified namespace from being registered.</remarks>
    /// <usage>[assembly: WhenRegisteringExcludeClassesInNamespace("YOUR_NAMESPACE")]</usage>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class WhenRegisteringExcludeClassesInNamespaceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhenRegisteringExcludeClassesInNamespaceAttribute" /> class.
        /// </summary>
        /// <param name="namespace">The namespace to exclude.</param>
        public WhenRegisteringExcludeClassesInNamespaceAttribute(string @namespace)
        {
            Namespace = @namespace;
        }

        /// <summary>
        /// Gets the namespace to exclude.
        /// </summary>
        public string Namespace { get; }
    }
}