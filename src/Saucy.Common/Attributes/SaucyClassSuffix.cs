using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class SaucyClassSuffix : Attribute
	{
		public SaucyClassSuffix(string suffix)
		{
			Suffix = suffix;
		}

		public string Suffix { get; private set; }
	}
}
