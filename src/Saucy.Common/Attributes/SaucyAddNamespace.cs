using System;
using System.Diagnostics.CodeAnalysis;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to tell Saucy to include all classes in the specified namespace in the service collection.
    /// You also need to specify the scope of the service to be added
    /// </summary>
    /// <usage>[assembly: SaucyAddNamespace(nameof(Saucy.Console.Builders), ServiceScope.Transient)]</usage>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Not applicable")]
    public sealed class SaucyAddNamespace : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaucyAddNamespace" /> class.
        /// </summary>
        /// <param name="namespace">The namespace to include.</param>
        /// <param name="scope">The default scope of the service to be added.</param>
        public SaucyAddNamespace(string @namespace, ServiceScope scope)
        {
            Namespace = @namespace;
            Scope = scope;
        }

        /// <summary>
        /// Gets the namespace to include.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Gets the default scope of the service to be added.
        /// </summary>
        public ServiceScope Scope { get; }
    }
}