namespace Saucy.Console.Generics;

[SaucyInclude(ServiceScope.Transient)]
[SaucyKeyedService("KeyedBig")]
public class OpenGenericTypeWithInterface<T> : IGenericInterface<T>;