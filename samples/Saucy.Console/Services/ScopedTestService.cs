using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Console.Services;

[SaucyClassScope(ServiceScope.Scoped)]
public class ScopedTestService : ITestService
{
	public void WriteToConsole()
	{
		System.Console.WriteLine("ScopedTestService");
	}
}
