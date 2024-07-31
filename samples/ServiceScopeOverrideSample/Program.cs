// Check the generated code.

using Microsoft.Extensions.DependencyInjection;

using ServiceScopeOverrideSample.ServiceCollectionExtensions;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddServiceScopeOverrideSampleServices();

// Notice that everything is the namespace (AssemblyInfo.cs) is registered with the scope Singleton, but ClassTwo has
// [SaucyInclude(ServiceScope.Transient)], which means it'll be registered as Transient.
// If you find yourself doing this often, you might want to consider the design of your application.

Console.ReadLine();