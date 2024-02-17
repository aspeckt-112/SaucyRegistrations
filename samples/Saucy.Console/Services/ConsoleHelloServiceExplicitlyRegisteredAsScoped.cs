using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Console.Services;

[UseScope(ServiceScope.Scoped)]
public class ConsoleHelloServiceExplicitlyRegisteredAsScoped : IConsoleHelloService
{

    public void SayHello()
    {
        throw new NotImplementedException();
    }
}