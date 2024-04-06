namespace Saucy.Console.AbstractRegistration;

[SaucyInclude(ServiceScope.Transient)]
[SaucyOnlyRegisterInterface]
public class SomethingUsingTheAbstractBaseClassAndTheInterface : AbstractRegistrationBaseClass, ISomeInterface
{

}