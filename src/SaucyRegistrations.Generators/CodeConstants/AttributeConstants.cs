namespace SaucyRegistrations.Generators.CodeConstants
{
    internal static class AttributeConstants
    {
        internal const string SaucyIncludeNamespaceWithSuffixAttributeName = "SaucyIncludeNamespaceWithSuffix";

        internal const string SaucyIncludeNamespaceAttribute = $$"""
                                                                 [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
                                                                 public class {{SaucyIncludeNamespaceWithSuffixAttributeName}} : System.Attribute
                                                                 {
                                                                     public {{SaucyIncludeNamespaceWithSuffixAttributeName}}(string suffix, ServiceScope defaultScope) { }
                                                                 }
                                                                 """;

        internal const string SaucyIncludeAttributeName = "SaucyInclude";

        internal const string SaucyIncludeAttribute = $$"""
                                                        [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                                        public class {{SaucyIncludeAttributeName}} : System.Attribute
                                                        {
                                                            public {{SaucyIncludeAttributeName}}(ServiceScope withScope) { }
                                                        }
                                                        """;
    }
}