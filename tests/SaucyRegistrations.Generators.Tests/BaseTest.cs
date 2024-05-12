using Microsoft.CodeAnalysis.Testing;

using GeneratorTest =
    Microsoft.CodeAnalysis.CSharp.Testing.CSharpSourceGeneratorTest<
        SaucyRegistrations.Generators.Tests.TestAdapter<SaucyRegistrations.Generators.SaucyGenerator>,
        Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace SaucyRegistrations.Generators.Tests;

public abstract class BaseTest
{
    private const string AttributeSource = """
                                           [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                           internal class SaucyInclude : System.Attribute
                                           {
                                               internal SaucyInclude(ServiceScope withScope) { }
                                           }

                                           [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
                                           internal class SaucyIncludeNamespaceWithSuffix : System.Attribute
                                           {
                                               internal SaucyIncludeNamespaceWithSuffix(string suffix, ServiceScope defaultScope) { }
                                           }

                                           [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                           internal class SaucyRegisterAbstractClass : System.Attribute
                                           {
                                               internal SaucyRegisterAbstractClass() { }
                                           }

                                           [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
                                           internal class SaucyDoNotRegisterWithInterface : System.Attribute
                                           {
                                               internal SaucyDoNotRegisterWithInterface(string @interface) { }
                                           }


                                           """;

    private const string EnumSource = """
                                      internal enum ServiceScope
                                      {
                                          Singleton = 0,
                                          Transient = 1,
                                          Scoped = 2
                                      }
                                      """;

    protected Task RunGenerator(string input, string expectedOutput)
    {
        return new GeneratorTest
        {
            ReferenceAssemblies = ReferenceAssemblies
                .Net
                .Net60
                .AddPackages([
                    new PackageIdentity("Microsoft.Extensions.DependencyInjection", "8.0.0")
                ]),
            TestState =
            {
                Sources = { input },
                GeneratedSources =
                {
                    (typeof(TestAdapter<SaucyGenerator>), "Saucy.Attributes.g.cs", Normalize(AttributeSource)),
                    (typeof(TestAdapter<SaucyGenerator>), "Saucy.Enums.g.cs", Normalize(EnumSource)),
                    (typeof(TestAdapter<SaucyGenerator>),
                        "TestProject.TestProjectServiceCollectionExtensions.g.cs", Normalize(expectedOutput))
                }
            }
        }.RunAsync();
    }

    private static string Normalize(string input)
    {
        return input.Replace("\r\n", "\n");
    }
}