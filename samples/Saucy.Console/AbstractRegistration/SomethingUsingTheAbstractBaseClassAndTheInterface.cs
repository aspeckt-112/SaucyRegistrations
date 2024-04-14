namespace Saucy.Console.AbstractRegistration;

[SaucyInclude(ServiceScope.Transient)]
public class SomethingUsingTheAbstractBaseClassAndTheInterface : AbstractRegistrationBaseClass, ISomeInterface
{

}