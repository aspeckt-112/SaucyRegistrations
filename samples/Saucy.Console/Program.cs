using Microsoft.Extensions.DependencyInjection;

using Saucy.Common.Attributes;

namespace Saucy.Console;

public static partial class Program
{
    public static void Main(string[] args)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        AddRegistrations(serviceCollection);
    }

    [SaucyRegistrationTarget]
    static partial void AddRegistrations(IServiceCollection serviceCollection);
}