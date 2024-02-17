using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="ExcludeNamespace" /> attribute is used to specify the namespaces to exclude from the registration process.
    /// If applied, classes within the specified namespaces will not be registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExcludeNamespace : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeNamespace" /> class.
        /// </summary>
        /// <param name="namespace">The namespace to exclude.</param>
        public ExcludeNamespace(string @namespace)
        {
            Namespace = @namespace;
        }

        /// <summary>
        /// Gets the namespace to exclude.
        /// </summary>
        public string Namespace { get; }
    }
}