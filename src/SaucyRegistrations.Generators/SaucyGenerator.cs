using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Builders;
using SaucyRegistrations.Generators.Collections;
using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;

namespace SaucyRegistrations.Generators;

/// <summary>
/// The source generator for the Saucy library.
/// </summary>
[Generator]
public class SaucyGenerator : ISourceGenerator
{
    /// <summary>
    /// The initialization method for the source generator.
    /// </summary>
    /// <remarks>Not used in this source generator.</remarks>
    /// <param name="context">The generator initialization context.</param>
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        RunConfiguration? runParameters = BuildRunConfiguration(context);

        if (runParameters is null)
        {
            // TODO Should I do something here?
            return;
        }

        var source = GenerateRegistrationCode(runParameters, context.Compilation.ObjectType);

        context.AddSource($"{runParameters.GenerationConfiguration.Class}.Generated.cs", source);
    }

    private RunConfiguration? BuildRunConfiguration(GeneratorExecutionContext context)
    {
        IAssemblySymbol compilationAssembly = context.Compilation.Assembly;

        var compilationAssemblyNamespaces = compilationAssembly.GlobalNamespace.GetNamespaces().ToList();

        if (compilationAssemblyNamespaces.Count == 0)
        {
            throw new InvalidOperationException("No namespaces found in the compilation assembly.");
        }

        GenerationConfiguration? generationConfiguration = GenerationConfigurationBuilder.Build(compilationAssemblyNamespaces);

        if (generationConfiguration is null)
        {
            throw new InvalidOperationException("No generation configuration found. Have you applied the [GenerateServiceCollectionMethod] attribute to a class?");
        }

        var assemblyCollection = new Assemblies();

        if (compilationAssembly.ShouldBeIncludedInSourceGeneration())
        {
            AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyScanConfiguration(compilationAssembly);
            assemblyCollection.Add(compilationAssembly).WithConfiguration(assemblyScanConfiguration);
        }

        List<IAssemblySymbol> referencedAssemblies = context
                                                     .Compilation
                                                     .SourceModule
                                                     .ReferencedAssemblySymbols
                                                     .Where(x => x.ShouldBeIncludedInSourceGeneration())
                                                     .ToList();

        foreach (IAssemblySymbol assembly in referencedAssemblies)
        {
            AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyScanConfiguration(assembly);
            assemblyCollection.Add(assembly).WithConfiguration(assemblyScanConfiguration);
        }

        // At this point, if there's nothing in the map then there's nothing to generate. So bail out.
        if (assemblyCollection.IsEmpty)
        {
            return null;
        }

        TypeSymbols allTypeSymbolsInAllAssemblies = GetAllTypeSymbolsInAllAssemblies(assemblyCollection);

        RunConfiguration runParameter = new(generationConfiguration, allTypeSymbolsInAllAssemblies);

        return runParameter;
    }

    private AssemblyScanConfiguration BuildAssemblyScanConfiguration(IAssemblySymbol assemblySymbol)
    {
        AssemblyScanConfiguration result = new();

        List<string> excludedNamespaces = assemblySymbol.GetExcludedNamespaces();

        if (excludedNamespaces.Count > 0)
        {
            result.ExcludedNamespaces.AddRange(excludedNamespaces);
        }

        result.IncludeMicrosoftNamespaces = assemblySymbol.ShouldIncludeMicrosoftNamespaces();
        result.IncludeSystemNamespaces = assemblySymbol.ShouldIncludeSystemNamespaces();
        result.DefaultServiceScope = assemblySymbol.GetDefaultServiceScope();

        return result;
    }

    private TypeSymbols GetAllTypeSymbolsInAllAssemblies(Assemblies assemblies)
    {
        TypeSymbols result = new();

        foreach (var assembly in assemblies)
        {
            IAssemblySymbol? assemblySymbol = assembly.Key;
            AssemblyScanConfiguration? scanConfiguration = assembly.Value;

            List<INamespaceSymbol> namespaces = GetNamespacesFromAssembly(assemblySymbol, scanConfiguration);

            if (namespaces.Count == 0)
            {
                continue;
            }

            foreach (INamespaceSymbol? @namespace in namespaces)
            {
                IEnumerable<(ITypeSymbol, ServiceScope)> typeSymbols = GetTypeSymbolsFromNamespace(@namespace, assemblySymbol, scanConfiguration);

                foreach ((ITypeSymbol typeSymbol, ServiceScope serviceScope) in typeSymbols)
                {
                    result.Add(typeSymbol, serviceScope);
                }
            }
        }

        return result;
    }

    private List<INamespaceSymbol> GetNamespacesFromAssembly(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyScanConfiguration)
    {
        List<INamespaceSymbol> namespaceSymbols = [];

        INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
        List<string> excludedNamespaces = assemblyScanConfiguration.ExcludedNamespaces;
        var includeMicrosoftNamespaces = assemblyScanConfiguration.IncludeMicrosoftNamespaces;
        var includeSystemNamespaces = assemblyScanConfiguration.IncludeSystemNamespaces;

        foreach (INamespaceSymbol @namespace in globalNamespace.GetNamespaces())
        {
            string namespaceName = @namespace.ToDisplayString();

            if (excludedNamespaces.Contains(namespaceName)
                || (namespaceName.StartsWith("Microsoft") && !includeMicrosoftNamespaces)
                || (namespaceName.StartsWith("System") && !includeSystemNamespaces))
            {
                continue;
            }

            namespaceSymbols.Add(@namespace);
        }

        return namespaceSymbols;
    }

    private IEnumerable<(ITypeSymbol, ServiceScope)> GetTypeSymbolsFromNamespace(
        INamespaceSymbol namespaceSymbol,
        IAssemblySymbol assemblySymbol,
        AssemblyScanConfiguration assemblyScanConfiguration)
    {
        List<(ITypeSymbol, ServiceScope)> result = [];

        List<INamedTypeSymbol> concreteNamedTypeSymbols = namespaceSymbol.GetTypeMembers().Where(x => !x.IsAbstract).Where(x => !x.IsStatic).ToList();

        if (concreteNamedTypeSymbols.Count == 0)
        {
            return result;
        }

        // Check to see if the assembly has a GenerateRegistrationForClassesWithSuffixAttribute attribute.
        // If it does, then only include types that end with the suffix.
        // If it doesn't, then include all types.
        List<string> suffixes = assemblySymbol.GetListOfStringsFromAttributeOnSymbol(
            nameof(GenerateRegistrationForClassesWithSuffixAttribute),
            nameof(GenerateRegistrationForClassesWithSuffixAttribute.Suffix));

        bool assemblyHasOneOrMoreClassSuffix = suffixes.Count > 0;

        foreach (INamedTypeSymbol namedTypeSymbol in concreteNamedTypeSymbols)
        {
            // Should the type be excluded from registration?
            if (namedTypeSymbol.GetFirstAttributeWithNameOrNull(nameof(WhenRegisteringWithContainerShouldExcludedAttribute)) is not null)
            {
                continue;
            }

            ServiceScope? namedTypeServiceScope = null;

            AttributeData? saucyServiceScopeAttribute = namedTypeSymbol.GetFirstAttributeWithNameOrNull(nameof(WhenRegisteringUseScope));

            if (saucyServiceScopeAttribute is not null)
            {
                namedTypeServiceScope = saucyServiceScopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(WhenRegisteringUseScope.ServiceScope));
            }

            ServiceScope serviceScope = namedTypeServiceScope ?? assemblyScanConfiguration.DefaultServiceScope;

            if (assemblyHasOneOrMoreClassSuffix)
            {
                foreach (string suffix in suffixes)
                {
                    if (namedTypeSymbol.Name.EndsWith(suffix))
                    {
                        result.Add((namedTypeSymbol, serviceScope));
                    }
                }

                continue;
            }

            result.Add((namedTypeSymbol, serviceScope));
        }

        return result;
    }

    private string GenerateRegistrationCode(RunConfiguration runConfiguration, INamedTypeSymbol objectSymbol)
    {
        StringBuilder sourceBuilder = new();

        GenerationConfiguration generationConfiguration = runConfiguration.GenerationConfiguration;

        sourceBuilder.Append(
            $@"//<auto-generated by Saucy on {DateTime.Now} />
using Microsoft.Extensions.DependencyInjection;

namespace {generationConfiguration.Namespace}
{{
	public static partial class {generationConfiguration.Class}
	{{
		public static void {generationConfiguration.Method}(IServiceCollection serviceCollection)
		{{");

        sourceBuilder.AppendLine();

        Dictionary<ServiceScope, string> serviceScopeToMethodNameMap = new()
        {
            { ServiceScope.Singleton, "serviceCollection.AddSingleton" },
            { ServiceScope.Scoped, "serviceCollection.AddScoped" },
            { ServiceScope.Transient, "serviceCollection.AddTransient" }
        };

        foreach (KeyValuePair<ITypeSymbol, ServiceScope> type in runConfiguration.TypesToRegister)
        {
            ITypeSymbol? typeSymbol = type.Key;
            ServiceScope typeScope = type.Value;

            string fullyQualifiedTypeName = typeSymbol.ToDisplayString();

            INamedTypeSymbol? classBaseType = typeSymbol.BaseType;

            bool classHasBaseType = classBaseType is not null && !ReferenceEquals(classBaseType, objectSymbol);

            if (classHasBaseType && typeSymbol.BaseType!.IsAbstract)
            {
                string fullyQualifiedBaseTypeName = typeSymbol.BaseType.ToDisplayString();

                sourceBuilder.AppendLine($@"            {serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedBaseTypeName}, {fullyQualifiedTypeName}>();");
            }

            bool classHasInterfaces = typeSymbol.Interfaces.Length > 0;

            switch (classHasInterfaces)
            {
                case true:
                {
                    foreach (INamedTypeSymbol @interface in typeSymbol.Interfaces)
                    {
                        string fullyQualifiedInterfaceName = @interface.ToDisplayString();

                        sourceBuilder.AppendLine($@"            {serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedInterfaceName}, {fullyQualifiedTypeName}>();");
                    }

                    break;
                }
                case false when !classHasBaseType:
                    sourceBuilder.AppendLine($@"            {serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedTypeName}>();");

                    break;
            }
        }

        sourceBuilder.AppendLine(@"        }");
        sourceBuilder.AppendLine(@"    }");
        sourceBuilder.AppendLine(@"}");

        return sourceBuilder.ToString();
    }
}