namespace SaucyRegistrations.Generators.SourceConstants.Enums;

/// <summary>
/// The definition of the ServiceScope enum.
/// </summary>
internal static class ServiceScope
{
    /// <summary>
    /// The value of the Singleton scope.
    /// </summary>
    internal const int SingletonScopeValue = 0;

    /// <summary>
    /// The value of the Transient scope.
    /// </summary>
    internal const int TransientScopeValue = 1;

    /// <summary>
    /// The value of the Scoped scope.
    /// </summary>
    internal const int ScopedScopeValue = 2;

    /// <summary>
    /// The definition of the ServiceScope enum.
    /// </summary>
    internal static readonly string ServiceScopeEnumDefinition = $$"""
                                                                   internal enum {{nameof(ServiceScope)}}
                                                                   {
                                                                       Singleton = {{SingletonScopeValue}},
                                                                       Transient = {{TransientScopeValue}},
                                                                       Scoped = {{ScopedScopeValue}}
                                                                   }
                                                                   """;
}