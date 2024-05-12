# Saucy Registrations

Do you always forget to register your services with the DI container? I do. So I wrote this, because I'd rather spend a lot of time writing a library than a little time remembering to do something that I really should be doing anyway.

## What is this?

Saucy is an [incremental source generator](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md). It provides a few attributes and an enum. Using these, it'll then generate a class within the same assembly that contains all the registrations for your services. You simply call the extension method it generates in your composition root and forget about it.


## How do I use it?

At it's core, usage is as simple as adding a single attribute to your service class. For example:

```csharp
[SaucyInclude(ServiceScope.Singleton)]
public class ExampleClass : IExample
{
}
```

Which is turn, generates the following code:

```csharp
namespace YOUR_PROJECT.ServiceCollectionExtensions;

public static class YOUR_PROJECTServiceCollectionExtensions
{
    public static void AddYOUR_PROJECTServices(IServiceCollection services)
    {
        services.AddSingleton<IExample, ExampleClass>();
    }
}
```

You can then call this method in your composition root:

```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddYOUR_PROJECTServices();
```

In essence, that's it. 

# MORE DOCUMENTATION TO COME