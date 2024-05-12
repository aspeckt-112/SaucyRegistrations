namespace Saucy.Console.AbstractRegistration;

[SaucyInclude(ServiceScope.Transient)]
[SaucyRegisterAbstractClass]
public class SomethingUsingTheAbstractBaseClassAndTheInterface : AbstractRegistrationBaseClass, ISomeInterface
{

}