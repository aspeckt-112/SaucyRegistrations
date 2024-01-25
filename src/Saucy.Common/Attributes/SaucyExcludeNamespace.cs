using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class SaucyExcludeNamespace : Attribute
	{
		public SaucyExcludeNamespace(string @namespace)
		{
			Namespace = @namespace;
		}

		public string Namespace { get; set; }
	}
}
