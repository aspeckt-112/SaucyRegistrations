using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Console.Services;

[SaucyClassScope(ServiceScope.Singleton)]
public class SingletonTestService : ITestService
{
	public void WriteToConsole()
	{
		System.Console.WriteLine("SingletonTestService");
	}
}
