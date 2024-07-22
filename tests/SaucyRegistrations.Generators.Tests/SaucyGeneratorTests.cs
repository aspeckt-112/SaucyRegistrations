using System.Runtime.InteropServices;

namespace SaucyRegistrations.Generators.Tests;

public class SaucyGeneratorTests : BaseTest
{
    [Fact]
    public async Task When_Empty_Should_GenerateBoilerplateOutput()
    {
        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator("", expected);
    }

    [Fact]
    public async Task SaucyInclude_When_SingleClass_With_TransientServiceScope_Should_GenerateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Transient)]
                             public class TestClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.TestClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyInclude_When_SingleClass_With_ScopedServiceScope_Should_GenerateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Scoped)]
                             public class TestClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddScoped<SaucyRegistrations.TestProject.TestClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyInclude_When_SingleClass_With_SingletonServiceScope_Should_GenerateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             public class TestClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton<SaucyRegistrations.TestProject.TestClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyInclude_When_MultipleClasses_With_DifferentServiceScopes_Should_GenerateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Transient)]
                             public class TestClass1 { }

                             [SaucyInclude(ServiceScope.Scoped)]
                             public class TestClass2 { }

                             [SaucyInclude(ServiceScope.Singleton)]
                             public class TestClass3 { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.TestClass1>();
                                        services.AddScoped<SaucyRegistrations.TestProject.TestClass2>();
                                        services.AddSingleton<SaucyRegistrations.TestProject.TestClass3>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        SaucyDoNotRegisterWithInterface_When_SingleClass_With_Interface_Should_RegisterClassWithoutInterface()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyDoNotRegisterWithInterface(nameof(ISomeInterface))]
                             [SaucyInclude(ServiceScope.Singleton)]
                             public class TestClass : ISomeInterface { }

                             public interface ISomeInterface { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton<SaucyRegistrations.TestProject.TestClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        SaucyDoNotRegisterWithInterface_When_SingleClass_With_MultipleInterfaces_Should_RegisterClassWithoutInterfaces()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyDoNotRegisterWithInterface(nameof(ISomeInterface))]
                             [SaucyDoNotRegisterWithInterface(nameof(ISomeOtherInterface))]
                             [SaucyInclude(ServiceScope.Singleton)]
                             public class TestClass : ISomeInterface, ISomeOtherInterface { }

                             public interface ISomeInterface { }
                             public interface ISomeOtherInterface { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton<SaucyRegistrations.TestProject.TestClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        SaucyDoNotRegisterWithInterface_When_SingleClass_With_MultipleInterfaces_And_OneInterfaceIsExcluded_And_AnotherIsNot_Should_RegisterClassWithoutExcludedInterface()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyDoNotRegisterWithInterface(nameof(ISomeOtherInterface))]
                             [SaucyInclude(ServiceScope.Singleton)]
                             public class TestClass : ISomeInterface, ISomeOtherInterface { }

                             public interface ISomeInterface { }
                             public interface ISomeOtherInterface { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton<SaucyRegistrations.TestProject.ISomeInterface, SaucyRegistrations.TestProject.TestClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyIncludeNamespace_When_NoClassHasServiceScopeOverride_Should_GenerateOutput()
    {
        const string input = """
                             [assembly: SaucyIncludeNamespace(nameof(SaucyRegistrations.TestProject.Repositories), ServiceScope.Transient)]

                             namespace SaucyRegistrations.TestProject.Repositories;

                             public class FirstExampleRepository : IRepository { }

                             public class SecondExampleRepository : IRepository { }

                             public class ThirdExampleRepository : IRepository { }

                             public interface IRepository { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.FirstExampleRepository>();
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.SecondExampleRepository>();
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.ThirdExampleRepository>();
                                        return services;
                                    }
                                }
                                """;


        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyIncludeNamespace_When_ClassHasServiceScopeOverride_Should_GenerateOutput()
    {
        const string input = """
                             [assembly: SaucyIncludeNamespace(nameof(SaucyRegistrations.TestProject.Repositories), ServiceScope.Transient)]

                             namespace SaucyRegistrations.TestProject.Repositories;

                             public class FirstExampleRepository : IRepository { }

                             public class SecondExampleRepository : IRepository { }

                             [SaucyInclude(ServiceScope.Scoped)]
                             public class ThirdExampleRepository : IRepository { }

                             public interface IRepository { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.FirstExampleRepository>();
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.SecondExampleRepository>();
                                        services.AddScoped<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.ThirdExampleRepository>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyRegisterAbstractClass_When_AttributeAttachedToConcreteClass_Should_RegisterAbstractClass()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             public abstract class AbstractClass { }

                             [SaucyInclude(ServiceScope.Transient)]
                             [SaucyRegisterAbstractClass]
                             public class ConcreteClass : AbstractClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.AbstractClass, SaucyRegistrations.TestProject.ConcreteClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        SaucyRegisterAbstractClass_When_ConcreteClassImplementingAbstractClass_But_NotMarkedWithSaucyRegisterAbstractClass_Should_NotRegisterAbstractClass()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             public abstract class AbstractClass { }

                             [SaucyInclude(ServiceScope.Transient)]
                             public class ConcreteClass : AbstractClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.ConcreteClass>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyExclude_When_SingleClass_Should_NotGenerateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyExclude]
                             public class TestClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    public async Task SaucyExclude_When_UsingSaucyIncludeNamespace_SpecificClassShouldBeExcluded()
    {
        const string input = """
                             [assembly: SaucyIncludeNamespace(nameof(SaucyRegistrations.TestProject.Repositories), ServiceScope.Transient)]

                             namespace SaucyRegistrations.TestProject.Repositories;

                             [SaucyExclude]
                             public class FirstExampleRepository : IRepository { }

                             public class SecondExampleRepository : IRepository { }

                             public class ThirdExampleRepository : IRepository { }

                             public interface IRepository { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.SecondExampleRepository>();
                                        services.AddTransient<SaucyRegistrations.TestProject.Repositories.IRepository, SaucyRegistrations.TestProject.Repositories.ThirdExampleRepository>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyExclude_When_MultipleClasses_Should_NotGenerateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyExclude]
                             public class TestClass1 { }

                             [SaucyExclude]
                             public class TestClass2 { }

                             [SaucyExclude]
                             public class TestClass3 { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        SaucyExclude_When_MultipleClasses_And_OneClassIsExcluded_Should_NotGenerateOutputForExcludedClass()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyExclude]
                             public class TestClass1 { }

                             [SaucyInclude(ServiceScope.Transient)]
                             public class TestClass2 { }

                             [SaucyInclude(ServiceScope.Transient)]
                             public class TestClass3 { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient<SaucyRegistrations.TestProject.TestClass2>();
                                        services.AddTransient<SaucyRegistrations.TestProject.TestClass3>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyKeyedService_When_KeyIsEmptyString_Then_ReturnBoilerplateOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyKeyedService("")]
                             public class KeyedService { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyKeyedService_When_KeyIsNotEmptyString_Then_ReturnKeyedServiceOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             [SaucyKeyedService("KeyedService")]
                             public class KeyedService { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddKeyedSingleton<SaucyRegistrations.TestProject.KeyedService>("KeyedService");
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task SaucyKeyedService_When_MultipleKeyedServices_Then_ReturnMultipleKeyedServicesOutput()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             [SaucyKeyedService("KeyedService1")]
                             public class KeyedService1 { }

                             [SaucyInclude(ServiceScope.Singleton)]
                             [SaucyKeyedService("KeyedService2")]
                             public class KeyedService2 { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddKeyedSingleton<SaucyRegistrations.TestProject.KeyedService1>("KeyedService1");
                                        services.AddKeyedSingleton<SaucyRegistrations.TestProject.KeyedService2>("KeyedService2");
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        Generics_When_InterfaceIsGeneric_With_TypeParameters_But_ClassIsNotGeneric_Should_RegisterClassWithInterface()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             public class TypeImplementingGenericInterface : IGenericInterface<string> { }

                             public interface IGenericInterface<T> { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton<SaucyRegistrations.TestProject.IGenericInterface<System.String>, SaucyRegistrations.TestProject.TypeImplementingGenericInterface>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        Generics_When_InterfaceIsGeneric_With_TypeParameters_And_ClassIsGeneric_Should_RegisterClassWithInterface()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Transient)]
                             public class AVeryGenericInterface<T1, T2, T3> : IAVeryGenericInterface<T1, T2, T3> { }

                             public interface IAVeryGenericInterface<T1, T2, T3> { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddTransient(typeof(SaucyRegistrations.TestProject.IAVeryGenericInterface<,,>), typeof(SaucyRegistrations.TestProject.AVeryGenericInterface<,,>));
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        Generics_When_InterfaceIsGeneric_With_TypeParameters_And_ClassIsGeneric_And_RegisteredWithKey_Should_RegisterClassWithInterface()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Transient)]
                             [SaucyKeyedService("KeyedSomething")]
                             public class OpenGenericType<T> : IGenericInterface<T> { }

                             public interface IGenericInterface<T> { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddKeyedTransient(typeof(SaucyRegistrations.TestProject.IGenericInterface<>), "KeyedSomething", typeof(SaucyRegistrations.TestProject.OpenGenericType<>));
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task Generics_When_ClassIsGeneric_With_NoInterface_Should_RegisterOnlyClass()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             public class OpenGenericTypeWithInterface<T> { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton(typeof(SaucyRegistrations.TestProject.OpenGenericTypeWithInterface<>));
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task Generics_When_ClassIsKeyedGeneric_With_NoInterface_Should_RegisterOnlyKeyedClass()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Transient)]
                             [SaucyKeyedService("Key")]
                             public class OpenGenericType<T> { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddKeyedTransient(typeof(SaucyRegistrations.TestProject.OpenGenericType<>), "Key");
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task
        When_ClassIsMarkedWithSaucyInclude_And_SaucyExclude_Then_ExcludeWillTakeProminence_And_ClassWilLBeExcluded()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             [SaucyExclude]
                             public class TestClass { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }

    [Fact]
    public async Task ServiceWithVariance_Should_RegisterCorrectly()
    {
        const string input = """
                             namespace SaucyRegistrations.TestProject;

                             [SaucyInclude(ServiceScope.Singleton)]
                             public class CovariantService : ICovariantService<object> { }

                             public interface ICovariantService<out T> { }
                             """;

        const string expected = """
                                // <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />
                                using Microsoft.Extensions.DependencyInjection;

                                namespace TestProject.ServiceCollectionExtensions;

                                public static class TestProjectServiceCollectionExtensions
                                {
                                    public static IServiceCollection AddTestProjectServices(this IServiceCollection services)
                                    {
                                        services.AddSingleton<SaucyRegistrations.TestProject.ICovariantService<System.Object>, SaucyRegistrations.TestProject.CovariantService>();
                                        return services;
                                    }
                                }
                                """;

        await RunGenerator(input, expected);
    }
}