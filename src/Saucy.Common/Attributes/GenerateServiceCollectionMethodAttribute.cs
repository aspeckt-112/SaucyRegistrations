using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class GenerateServiceCollectionMethodAttribute : Attribute
	{
		public string MethodName { get; }

		public GenerateServiceCollectionMethodAttribute(string methodName)
		{
			MethodName = methodName;
		}
	}
}
