namespace SaucyRegistrations.Generators.Configurations
{
	public class GenerationConfiguration(string @namespace, string @class, string method)
	{
		public string Namespace { get; } = @namespace;

		public string Class { get; } = @class;

		public string Method { get; } = method;
	}
}
