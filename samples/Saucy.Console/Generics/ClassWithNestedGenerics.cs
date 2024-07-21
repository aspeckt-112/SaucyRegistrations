namespace Saucy.Console.Generics;

[SaucyInclude(ServiceScope.Scoped)]
public class ClassWithNestedGenerics : IGenericInterface<Dictionary<string, List<int>>>
{

}