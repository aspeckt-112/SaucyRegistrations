namespace Saucy.Console.Generics;

[SaucyInclude(ServiceScope.Transient)]
public class OpenGenericTypeWithInterface<T> : IGenericInterface<T>;