using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class GenerateRegistrationForClassesWithSuffixAttribute : Attribute
	{
		public GenerateRegistrationForClassesWithSuffixAttribute(string suffix)
		{
			Suffix = suffix;
		}

		public string Suffix { get; private set; }
	}
}
