using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="ServiceCollectionMethod" /> attribute is used to specify the method that will be generated for the
    /// service collection.
    /// If this is not applied to a class in the common assembly, the source generation process will not occur.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceCollectionMethod : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCollectionMethod" /> class.
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