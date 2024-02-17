using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Console.Services;

[UseScope(ServiceScope.Singleton)]
public class HelloExplicitlyRegisteredAsSingletonService : IConsoleHelloService
{
    public void SayHello()
    {
        throw new NotImplementedException();
    }
}