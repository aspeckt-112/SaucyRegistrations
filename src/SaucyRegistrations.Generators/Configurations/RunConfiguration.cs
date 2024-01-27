namespace SaucyRegistrations.Generators.Configurations;

public class RunConfiguration(GenerationConfiguration generationConfiguration)
{
	public GenerationConfiguration GenerationConfiguration { get; } = generationConfiguration;
}