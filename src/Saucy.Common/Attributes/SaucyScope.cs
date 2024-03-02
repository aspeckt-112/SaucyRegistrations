using System;
using System.Diagnostics.CodeAnalysis;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to tell Saucy which service scope to use when adding the class to the service collection.
    /// If you're using <see cref="SaucyAddNamespace"/> you can specify the scope there. However, if you want to specify the scope for a single class, you can use this attribute to
    /// override the scope specified in <see cref="SaucyAddNamespace"/>.
    /// </summary>
    /// <usage>[SaucyScope(ServiceScope.Singleton)]<para/>public class SingletonClass{}</usage>
    [AttributeUsage(AttributeTargets.Class)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Not applicable")]
    public sealed class SaucyScope : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaucyScope"/> class.
        /// </summary>
        /// <param name="serviceScope">The <see cref="ServiceScope"/> to use when adding the class to the service collection.</param>
        public SaucyScope(ServiceScope serviceScope)
        {
            ServiceScope = serviceScope;
        }

        /// <summary>
        /// Gets the <see cref="ServiceScope"/> to use when adding the class to the service collection.
        /// </summary>
        public ServiceScope ServiceScope { get; }
    }
}