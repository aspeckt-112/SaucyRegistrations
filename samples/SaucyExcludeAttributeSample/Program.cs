using Microsoft.Extensions.DependencyInjection;

using SaucyExcludeAttributeSample.ServiceCollectionExtensions;

using SaucyExcludeSample.Classes;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSaucyExcludeAttributeSampleServices();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

// This will throw an exception because AClassToExclude is excluded
AClassToExclude aClassToExclude = serviceProvider.GetRequiredService<AClassToExclude>();

