using Microsoft.Extensions.DependencyInjection;

using SaucyExcludeSample.Classes;
using SaucyExcludeSample.ServiceCollectionExtensions;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSaucyExcludeSampleServices();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

// This will throw an exception because AClassToExclude is excluded
AClassToExclude aClassToExclude = serviceProvider.GetRequiredService<AClassToExclude>();

