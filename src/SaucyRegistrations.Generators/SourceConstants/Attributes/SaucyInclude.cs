namespace SaucyRegistrations.Generators.SourceConstants.Attributes
{
    /// <summary>
    /// The definition of the SaucyInclude attribute.
    /// </summary>
    internal static class SaucyInclude
    {
        /// <summary>
        /// The value of the SaucyInclude attribute.
        /// </summary>
        internal const string SaucyIncludeAttributeDefinition = $$"""
                                                                  [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                                                  internal class {{nameof(SaucyInclude)}} : System.Attribute
                                                                  {
                                                                      internal {{nameof(SaucyInclude)}}(ServiceScope withScope) { }
                                                                  }
                                                                  """;
    }
}