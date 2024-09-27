# Saucy Registrations

I've worked on a few large enterprise projects that made use of a dependency injection container. Across multiple different projects at various jobs, there existed some code to "automagically" register services with the container following some sort of convention. For example, classes that ended with "Repository" would be registered as a singleton, while classes that ended with "Service" would be registered as transient, etc. This code was always done with reflection, which was normally a pain to maintain and always seemed to be a bit of a black box.

I wanted to solve the problem for myself, once and for all. I wanted to solve it with minimal performance impact, minimal configuration, and maximum readability.

Enter...this.

## What is this?

Saucy is an [incremental source generator](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md). Within the assembly it's been installed into, it generates a class with a single `IServiceCollection` extension method. You can then choose to register all classes within a namespace at an assembly level, classes at an individual level, or a combination of both. Then, you simply call the generated extension method within your `Startup.cs` file, and all of your services will be registered with the container.

## How do I use this?

There's a comprehensive suite of integration tests within the solution, but here's a quick rundown.

The best way to make use of Saucy is to take advantage of the assembly-level namespace registration. Assume the following directory structure:

___

NamespaceOne:
- Class called `ServiceOne`
- Class called `ServiceTwo`

NamespaceTwo:
- Class called `ServiceThree`
- Class called `ServiceFour`

AssemblyInfo.cs

---

Within `AssemblyInfo.cs`, you can add the following attributes:

```csharp
[assembly: SaucyIncludeNamespace(nameof(SaucySample.NamespaceOne), ServiceScope.Transient)]
[assembly: SaucyIncludeNamespace(nameof(SaucySample.NamespaceTwo), ServiceScope.Scoped)]
```

Then, within your Startup.cs file, you can call the generated extension method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Add{YOURASSEMBLYNAME}Services();
}
```

Any new classes you add to the NamespaceOne namespace will automatically be registered as transient, and any new classes you add to the NamespaceTwo namespace will automatically be registered as scoped.

It's also possible to include classes at an individual level. It's as simple as:

```csharp
[SaucyInclude(ServiceScope.Singleton)]
public class YOURCLASS
{
    
}
```

### Additional Notes
- If your class implements an interface, the interface will be registered as the service, and the class will be registered as the implementation. If you wish to exclude the interface, you can use the SaucyDoNotRegisterWithInterface attribute at the class level.
- By default, abstract classes are not registered as services; only the class that implements them is registered. If you wish to change this, you can use the SaucyRegisterAbstractClass attribute at the class level. I can count on one hand the number of times I've needed to resolve an abstract class, but I've included this feature for completeness.
- If you're using namespace-level registration, you can exclude a class from being registered by using the SaucyExclude attribute at the class level. However, if you find yourself needing to do this often, it might indicate that your namespaces are too broad—at least from my experience.
- Both open and closed generics are supported. Please see the integration tests for examples.
- Keyed services are supported via the SaucyKeyedService attribute. Please see the integration tests for examples.

# Anything else?

I'm always open to feedback and contributions. If you have any ideas, please feel free to open an issue or a PR. I'm always looking to improve this library. Enjoy using Saucy!