// Global using directives

global using AssemblyName = Microsoft.CodeAnalysis.IncrementalValueProvider<string>;

global using CompilationProvider = Microsoft.CodeAnalysis.IncrementalValueProvider<Microsoft.CodeAnalysis.Compilation>;

global using ServiceDefinitionsFromNamespace =
    Microsoft.CodeAnalysis.IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<
        SaucyRegistrations.Generators.Models.ServiceDefinition>>;

// LOL Gross
global using ServicesProvider =
    Microsoft.CodeAnalysis.IncrementalValueProvider<((
        System.Collections.Immutable.ImmutableArray<SaucyRegistrations.Generators.Models.ServiceDefinition> Services,
        string AssemblyName) ExplicitlyRegisteredServices,
        System.Collections.Immutable.ImmutableArray<SaucyRegistrations.Generators.Models.ServiceDefinition>
        NamespaceRegisteredServices)>;
