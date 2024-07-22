using Microsoft.Extensions.DependencyInjection;

using SaucyIncludeNamespaceSample.NamespaceWithInterfaces;
using SaucyIncludeNamespaceSample.NamespaceWithoutInterfaces;
using SaucyIncludeNamespaceSample.ServiceCollectionExtensions;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSaucyIncludeNamespaceSampleServices();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

AClassWithoutAnInterface aClassWithoutAnInterface = serviceProvider.GetRequiredService<AClassWithoutAnInterface>();
IAnInterface anInterface = serviceProvider.GetRequiredService<IAnInterface>();

// No crash means success