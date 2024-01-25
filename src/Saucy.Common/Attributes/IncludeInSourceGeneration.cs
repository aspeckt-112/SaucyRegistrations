using System;
using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class IncludeInSourceGeneration : Attribute
	{
		public IncludeInSourceGeneration(ServiceScope defaultServiceScope)
		{
			DefaultServiceScope = defaultServiceScope;
		}

		public ServiceScope DefaultServiceScope { get; private set; }
	}
}
