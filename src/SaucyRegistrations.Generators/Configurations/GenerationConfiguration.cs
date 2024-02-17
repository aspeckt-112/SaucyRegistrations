namespace SaucyRegistrations.Generators.Configurations;

/// <summary>
/// The configuration for the generation process.
/// </summary>
/// <param name="namespace">The namespace the registration code will be generated in.</param>
/// <param name="class">The class the registration code will be generated in.</param>
/// <param name="method">The method that will be generated for the registration code.</param>
public class GenerationConfiguration(string @namespace, string @class, string method)
{
    /// <summary>
    /// Gets the namespace the registration code will be generated in.
    /// </summary>
    public string Namespace { get; } = @namespace;

    /// <summary>
    /// Gets the class the registration code will be generated in.
    /// </summary>
    public string Class { get; } = @class;

    /// <summary>
    /// Gets the method that will be generated for the registration code.
    /// </summary>
    public string Method { get; } = method;
}