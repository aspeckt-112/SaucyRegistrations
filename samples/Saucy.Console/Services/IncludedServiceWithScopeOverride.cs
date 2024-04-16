namespace Saucy.Console.Services;

[SaucyInclude(ServiceScope.Scoped)]
[SaucyDoNotRegisterWithInterface(nameof(IService))]
public class IncludedServiceWithScopeOverride : IService
{

}