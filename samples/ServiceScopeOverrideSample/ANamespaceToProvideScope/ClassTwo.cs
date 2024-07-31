namespace ServiceScopeOverrideSample.ANamespaceToProvideScope;

[SaucyInclude(ServiceScope.Transient)]
public class ClassTwo : IAnInterface
{

}