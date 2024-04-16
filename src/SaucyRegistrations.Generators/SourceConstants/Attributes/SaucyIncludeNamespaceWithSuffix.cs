namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyIncludeNamespaceWithSuffix attribute.
/// </summary>
internal static class SaucyIncludeNamespaceWithSuffix
{
    /// <summary>
    /// The value of the SaucyIncludeNamespaceWithSuffix attribute.
    /// </summary>
    internal const string SaucyIncludeNamespaceWithSuffixAttributeDefinition = $$"""
                                                                                 [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
                                                                                 internal class {{nameof(SaucyIncludeNamespaceWithSuffix)}} : System.Attribute
                                                                                 {
                                                                                     internal {{nameof(SaucyIncludeNamespaceWithSuffix)}}(string suffix, ServiceScope defaultScope) { }
                                                                                 }
                                                                                 """;
}