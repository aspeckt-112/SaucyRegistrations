using System;
using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute : Attribute
	{
		public IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute(ServiceScope defaultServiceScope)
		{
			DefaultServiceScope = defaultServiceScope;
		}

		public ServiceScope DefaultServiceScope { get; private set; }
	}
}
