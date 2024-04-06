namespace SaucyConcreteTypes.Library.AbstractRegistration;

[SaucyInclude(ServiceScope.Singleton)]
public class ClassWithMultipleGenericTypes : AbstractBaseClassWithMultipleGenericTypes<int, ComplexType>
{

}