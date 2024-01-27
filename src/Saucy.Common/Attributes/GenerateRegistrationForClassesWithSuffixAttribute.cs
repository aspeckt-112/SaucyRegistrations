using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class GenerateRegistrationForClassesWithSuffixAttribute : Attribute
	{
		public GenerateRegistrationForClassesWithSuffixAttribute(string suffix)
		{
			Suffix = suffix;
		}

		public string Suffix { get; private set; }
	}
}
