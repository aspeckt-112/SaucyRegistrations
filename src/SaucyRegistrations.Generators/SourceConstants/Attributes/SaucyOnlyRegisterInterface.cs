namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyOnlyRegisterInterface attribute.
/// </summary>
internal static class SaucyOnlyRegisterInterface
{
    /// <summary>
    /// The value of the SaucyOnlyRegisterInterface attribute.
    /// </summary>
    internal const string SaucyOnlyRegisterInterfaceAttributeDefinition = $$"""
                                                                            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                                                            internal class {{nameof(SaucyOnlyRegisterInterface)}} : System.Attribute
                                                                            {
                                                                                internal {{nameof(SaucyOnlyRegisterInterface)}}() { }
                                                                            }
                                                                            """;
}