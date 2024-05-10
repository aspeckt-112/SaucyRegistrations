// Global using directives

global using AssemblyAttributesProvider = Microsoft.CodeAnalysis.IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<SaucyRegistrations.Generators.Models.ServiceDefinition>>;
global using AssemblyNameProvider = Microsoft.CodeAnalysis.IncrementalValueProvider<string>;

// LOL Gross
global using ServicesProvider = Microsoft.CodeAnalysis.IncrementalValueProvider<((System.Collections.Immutable.ImmutableArray<SaucyRegistrations.Generators.Models.ServiceDefinition> Services, string AssemblyName) ExplicitlyRegisteredServices, System.Collections.Immutable.ImmutableArray<SaucyRegistrations.Generators.Models.ServiceDefinition> NamespaceRegisteredServices)>;
