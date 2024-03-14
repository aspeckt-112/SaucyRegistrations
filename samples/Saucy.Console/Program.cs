using Microsoft.Extensions.DependencyInjection;

using Saucy.Console.Services;

var serviceCollection = new ServiceCollection();

serviceCollection.AddSaucyConsoleServices();

var serviceProvider = serviceCollection.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<IncludedServiceOne>();


Console.ReadLine();