using Microsoft.Extensions.DependencyInjection;

using SaucyIncludeAttributeSample;
using SaucyIncludeAttributeSample.ServiceCollectionExtensions;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSaucyIncludeAttributeSampleServices();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

SaucyIncludeClassDirectly saucyIncludeClassDirectly = serviceProvider.GetRequiredService<SaucyIncludeClassDirectly>();
ISaucyIncludeInterface saucyIncludeInterface = serviceProvider.GetRequiredService<ISaucyIncludeInterface>();

// No crash means success