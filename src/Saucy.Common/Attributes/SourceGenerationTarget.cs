using System;

namespace Saucy.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class SourceGenerationTarget : Attribute
	{
		public SourceGenerationTarget(string @namespace, string partialClass, string generationMethod)
		{
			Namespace = @namespace;
			PartialClass = partialClass;
			GenerationMethod = generationMethod;
		}

		public string Namespace { get; private set; }

		public string PartialClass { get; private set; }

		public string GenerationMethod { get; private set; }
	}
}
