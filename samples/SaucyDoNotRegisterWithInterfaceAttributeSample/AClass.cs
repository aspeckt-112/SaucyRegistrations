namespace SaucyDoNotRegisterWithInterfaceAttributeSample;

[SaucyInclude(ServiceScope.Singleton)]
[SaucyDoNotRegisterWithInterface(nameof(IAnInterface))]
public class AClass : IAnInterface
{

}