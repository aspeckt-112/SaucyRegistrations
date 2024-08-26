namespace SaucyRegistrations.Generators.SourceConstants.Attributes;

/// <summary>
/// The definition of the SaucyDoNotRegisterInterface attribute.
/// </summary>
internal static class SaucyDoNotRegisterWithInterface
{
    /// <summary>
    /// The value of the SaucyDoNotRegisterInterface attribute.
    /// </summary>
    internal const string SaucyDoNotRegisterWithInterfaceDefinition = $$"""
                                                                        [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
                                                                        internal class {{nameof(SaucyDoNotRegisterWithInterface)}} : System.Attribute
                                                                        {
                                                                            internal {{nameof(SaucyDoNotRegisterWithInterface)}}(string @interface) { }
                                                                        }
                                                                        """;
}