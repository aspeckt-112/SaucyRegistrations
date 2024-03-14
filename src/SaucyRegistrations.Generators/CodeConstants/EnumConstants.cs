namespace SaucyRegistrations.Generators.CodeConstants
{
    internal static class EnumConstants
    {
        internal const string ServiceScopeEnumName = "ServiceScope";

        internal const int SingletonValue = 0;

        internal const int TransientValue = 1;

        internal const int ScopedValue = 2;

        internal static readonly string ServiceScopeEnum = $$"""
                                                             public enum {{ServiceScopeEnumName}}
                                                             {
                                                                 Singleton = {{SingletonValue}},
                                                                 Transient = {{TransientValue}},
                                                                 Scoped = {{ScopedValue}}
                                                             }
                                                             """;
    }
}