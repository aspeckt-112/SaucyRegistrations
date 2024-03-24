using Microsoft.Extensions.DependencyInjection;

using Saucy.Console.ServiceCollectionExtensions;
using Saucy.Console.Services;

using SaucyConcreteTypes.Library;
using SaucyConcreteTypes.Library.ServiceCollectionExtensions;

var serviceCollection = new ServiceCollection();

serviceCollection.AddSaucyConsoleServices();
serviceCollection.AddSaucyConcreteTypesLibraryServices();


var serviceProvider = serviceCollection.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<IService>();

var anotherService = serviceProvider.GetRequiredService<ConcreteSingleton>();

Console.ReadLine();