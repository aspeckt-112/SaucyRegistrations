using Microsoft.CodeAnalysis.Testing;

using SaucyRegistrations.Generators.Helpers;

// https://github.com/dotnet/roslyn-sdk/issues/1099#issuecomment-1723487931
using GeneratorTest =
    Microsoft.CodeAnalysis.CSharp.Testing.CSharpSourceGeneratorTest<
        SaucyRegistrations.Generators.Tests.TestAdapter<SaucyRegistrations.Generators.SaucyGenerator>,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

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
                                           internal class SaucyIncludeNamespace : System.Attribute
                                           {
                                               internal SaucyIncludeNamespace(string suffix, ServiceScope defaultScope) { }
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

                                           [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                           internal class SaucyExclude : System.Attribute
                                           {
                                               internal SaucyExclude() { }
                                           }

                                           [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                           internal class SaucyKeyedService : System.Attribute
                                           {
                                               internal SaucyKeyedService(string key) { }
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
                .Net80
                .AddPackages([
                    new PackageIdentity("Microsoft.Extensions.DependencyInjection", "8.0.0"),
                    new PackageIdentity("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                ]),
            TestState =
            {
                Sources = { input },
                GeneratedSources =
                {
                    (typeof(TestAdapter<SaucyGenerator>), "Saucy.Attributes.g.cs", StringHelpers.Normalize(AttributeSource)),
                    (typeof(TestAdapter<SaucyGenerator>), "Saucy.Enums.g.cs", StringHelpers.Normalize(EnumSource)),
                    (typeof(TestAdapter<SaucyGenerator>),
                        "TestProject.TestProjectServiceCollectionExtensions.g.cs", StringHelpers.Normalize(expectedOutput))
                }
            }
        }.RunAsync();
    }
}