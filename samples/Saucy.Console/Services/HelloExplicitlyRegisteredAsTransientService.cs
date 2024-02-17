using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Console.Services;

[UseScope(ServiceScope.Transient)]
public class HelloExplicitlyRegisteredAsTransientService : IConsoleHelloService
{
    public void SayHello()
    {
        throw new NotImplementedException();
    }
}