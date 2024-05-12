namespace SaucyConcreteTypes.Library.AbstractRegistration;

[SaucyInclude(ServiceScope.Singleton)]
[SaucyRegisterAbstractClass]
public class ClassWithMultipleGenericTypes : AbstractBaseClassWithMultipleGenericTypes<int, ComplexType>
{

}