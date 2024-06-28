namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyKeyedService attribute.
/// </summary>
internal static class SaucyKeyedService
{
    /// <summary>
    /// The value of the SaucyKeyedService attribute.
    /// </summary>
    internal const string SaucyKeyedServiceDefinition = $$"""
                                                          [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                                          internal class {{nameof(SaucyKeyedService)}} : System.Attribute
                                                          {
                                                              internal {{nameof(SaucyKeyedService)}}(string key) { }
                                                          }
                                                          """;
}