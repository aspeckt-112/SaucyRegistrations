using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public class SaucyIncludeMicrosoftNamespaces : Attribute { }
}
