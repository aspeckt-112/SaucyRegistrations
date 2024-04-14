namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyRegisterAbstractClass attribute.
/// </summary>
internal static class SaucyRegisterAbstractClass
{
    /// <summary>
    /// The value of the SaucyRegisterAbstractClass attribute.
    /// </summary>
    internal const string SaucyRegisterAbstractClassAttributeDefinition = $$"""
                                                                            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                                                            internal class {{nameof(SaucyRegisterAbstractClass)}} : System.Attribute
                                                                            {
                                                                                internal {{nameof(SaucyRegisterAbstractClass)}}() { }
                                                                            }
                                                                            """;
}