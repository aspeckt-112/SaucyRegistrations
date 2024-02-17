using System;

namespace Saucy.Common.Attributes
{
    /// <summary>
    /// The <see cref="SuffixRegistration" /> attribute is used to specify the suffix of the class, or classes, to register.
    /// This attribute is used to register classes that have a specific suffix. It can be applied multiple times to register
    /// multiple suffixes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class SuffixRegistration : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SuffixRegistration" /> class.</summary>
        /// <param name="suffix">The suffix of the class, or classes, to register.</param>
        public SuffixRegistration(string suffix)
        {
            Suffix = suffix;
        }

        /// <summary>
        /// Gets the suffix of the class, or classes.
        /// </summary>
        public string Suffix { get; }
    }
}