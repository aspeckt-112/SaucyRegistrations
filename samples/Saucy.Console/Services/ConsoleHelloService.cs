namespace Saucy.Console.Services;

public class ConsoleHelloService : IConsoleHelloService
{
	public void SayHello()
	{
		System.Console.WriteLine("Hello, World!");
	}
}
