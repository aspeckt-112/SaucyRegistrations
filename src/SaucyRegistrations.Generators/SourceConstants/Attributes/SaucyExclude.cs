namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyExclude attribute.
/// </summary>
internal static class SaucyExclude
{
    /// <summary>
    /// The value of the SaucyExclude attribute.
    /// </summary>
    internal const string SaucyExcludeAttributeDefinition = $$"""
                                                              [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                                              internal class {{nameof(SaucyExclude)}} : System.Attribute
                                                              {
                                                                  internal {{nameof(SaucyExclude)}}() { }
                                                              }
                                                              """;
}