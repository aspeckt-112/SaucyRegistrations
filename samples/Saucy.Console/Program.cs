using Microsoft.Extensions.DependencyInjection;

namespace Saucy.Console;

public static partial class Program
{

	public static void Main(string[] args)
	{
		ServiceCollection serviceCollection = new();
		AddRegistrations(serviceCollection);
	}

	private static void AddRegistrations(IServiceCollection serviceCollection)
	{
		AddRegistrationsInternal(serviceCollection);
	}
}
