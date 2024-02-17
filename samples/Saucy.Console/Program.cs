using Microsoft.Extensions.DependencyInjection;

using Saucy.Common.Attributes;

namespace Saucy.Console;

[ServiceCollectionMethod("AddRegistrations")]
public static partial class Program
{
    public static void Main(string[] args)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        AddRegistrations(serviceCollection);
    }
}