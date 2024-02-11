using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The GenerateServiceCollectionMethodAttribute is used to generate a method that adds services to the service collection.
    /// </summary>
    /// <remarks>Apply this attribute to the class that you'd like the service registration code to be generated for.</remarks>
    /// <usage>[GenerateServiceCollectionMethod("YOUR_METHOD_NAME")]</usage>
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateServiceCollectionMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateServiceCollectionMethodAttribute" /> class.
        /// </summary>
        /// <param name="methodName">The name of the method that will be generated.</param>
        public GenerateServiceCollectionMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Gets the name of the method that will be generated.
        /// </summary>
        public string MethodName { get; }
    }
}