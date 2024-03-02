using System;
using System.Diagnostics.CodeAnalysis;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// This attribute is used to identify the class that the service collection method will be generated for.
    /// </summary>
    /// <usage>[ServiceCollectionMethod("AddRegistrations")]<para />public partial class Program{}</usage>
    [AttributeUsage(AttributeTargets.Class)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Not applicable")]
    public sealed class ServiceCollectionMethod : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCollectionMethod"/> class.
        /// </summary>
        /// <param name="methodName">The name of the method that will be generated.</param>
        public ServiceCollectionMethod(string methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Gets the name of the method that will be generated.
        /// </summary>
        public string MethodName { get; }
    }
}