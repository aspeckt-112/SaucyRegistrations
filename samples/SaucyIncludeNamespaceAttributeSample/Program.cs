using Microsoft.Extensions.DependencyInjection;

using SaucyIncludeNamespaceAttributeSample.NamespaceWithInterfaces;
using SaucyIncludeNamespaceAttributeSample.NamespaceWithoutInterfaces;
using SaucyIncludeNamespaceAttributeSample.ServiceCollectionExtensions;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSaucyIncludeNamespaceAttributeSampleServices();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

AClassWithoutAnInterface aClassWithoutAnInterface = serviceProvider.GetRequiredService<AClassWithoutAnInterface>();
IAnInterface anInterface = serviceProvider.GetRequiredService<IAnInterface>();

// No crash means success