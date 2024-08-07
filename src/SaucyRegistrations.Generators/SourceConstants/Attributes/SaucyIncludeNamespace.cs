namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyIncludeNamespaceWithSuffix attribute.
/// </summary>
internal static class SaucyIncludeNamespace
{
    /// <summary>
    /// The value of the SaucyIncludeNamespaceWithSuffix attribute.
    /// </summary>
    internal const string SaucyIncludeNamespaceWithSuffixAttributeDefinition = $$"""
                                                                                 [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
                                                                                 internal class {{nameof(SaucyIncludeNamespace)}} : System.Attribute
                                                                                 {
                                                                                     internal {{nameof(SaucyIncludeNamespace)}}(string suffix, ServiceScope defaultScope) { }
                                                                                 }
                                                                                 """;
}