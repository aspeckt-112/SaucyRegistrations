namespace Saucy.Console.AbstractRegistration;

[SaucyInclude(ServiceScope.Scoped)]
[SaucyRegisterAbstractClass]
public class SomethingGenericAbstractRegistered : GenericAbstractRegistrationBaseClass<string>
{

}