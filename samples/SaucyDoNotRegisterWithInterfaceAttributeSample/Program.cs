using Microsoft.Extensions.DependencyInjection;

using SaucyDoNotRegisterWithInterfaceAttributeSample;
using SaucyDoNotRegisterWithInterfaceAttributeSample.ServiceCollectionExtensions;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSaucyDoNotRegisterWithInterfaceAttributeSampleServices();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

IAnInterface aClassWithoutAnInterface = serviceProvider.GetRequiredService<IAnInterface>();

// Crash means success - it can't be resolved because there's no registration for IAnInterface