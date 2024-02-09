﻿namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The GenerateRegistrationForClassesWithSuffixAttribute is used to generate registrations for classes with a
    /// specific suffix.
    /// </summary>
    /// <remarks>Apply this attribute to an assembly to include classes with the suffix that you specify.</remarks>
    /// <usage>[assembly: GenerateRegistrationForClassesWithSuffix("YOUR_SUFFIX")]</usage>
    [System.AttributeUsageAttribute(System.AttributeTargets.Assembly, AllowMultiple = true)]
    public class GenerateRegistrationForClassesWithSuffixAttribute : System.Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="GenerateRegistrationForClassesWithSuffixAttribute" /> class.</summary>
        /// <param name="suffix">The suffix of the class, or classes, that you want to register.</param>
        public GenerateRegistrationForClassesWithSuffixAttribute(string suffix)
        {
            Suffix = suffix;
        }

        /// <summary>
        /// Gets the suffix of the class, or classes.
        /// </summary>
        public string Suffix { get; }
    }
}