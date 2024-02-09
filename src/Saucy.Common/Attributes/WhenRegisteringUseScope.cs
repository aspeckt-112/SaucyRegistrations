using System;
using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class WhenRegisteringUseScope : Attribute
	{
		public WhenRegisteringUseScope(ServiceScope serviceScope)
		{
			ServiceScope = serviceScope;
		}

		public ServiceScope ServiceScope { get; private set; }
	}
}
