using System;
using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class SaucyClassScope : Attribute
	{
		public SaucyClassScope(ServiceScope serviceScope)
		{
			ServiceScope = serviceScope;
		}

		public ServiceScope ServiceScope { get; private set; }
	}
}
