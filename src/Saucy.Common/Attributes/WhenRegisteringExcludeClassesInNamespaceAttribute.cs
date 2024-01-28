using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class WhenRegisteringExcludeClassesInNamespaceAttribute : Attribute
	{
		public WhenRegisteringExcludeClassesInNamespaceAttribute(string @namespace)
		{
			Namespace = @namespace;
		}

		public string Namespace { get; set; }
	}
}
