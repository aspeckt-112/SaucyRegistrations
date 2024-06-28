﻿using Microsoft.Extensions.DependencyInjection;

using Saucy.Console.Keyed;
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

// Get keyed service
var keyedService = serviceProvider.GetRequiredKeyedService<IKeyed>("KeyedBig");

if (keyedService is not null)
{

}


Console.ReadLine();