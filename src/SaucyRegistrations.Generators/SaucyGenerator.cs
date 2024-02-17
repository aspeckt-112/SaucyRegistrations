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

using Type = SaucyRegistrations.Generators.Models.Type;

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
        var compilation = context.Compilation;

        IAssemblySymbol compilationAssembly = compilation.Assembly;

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

        // I think I hate how the service scope is being passed around here.
        if (compilationAssembly.ShouldBeIncludedInSourceGeneration(out ServiceScope? defaultServiceScope))
        {
            AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyScanConfiguration(compilationAssembly, defaultServiceScope!);
            assemblyCollection.Add(compilationAssembly).WithConfiguration(assemblyScanConfiguration);
        }

        List<(IAssemblySymbol? assembly, ServiceScope? serviceScope)> referencedAssemblies = context
                                                                                             .Compilation
                                                                                             .SourceModule
                                                                                             .ReferencedAssemblySymbols
                                                                                             .Select(a => a.ShouldBeIncludedInSourceGeneration(out ServiceScope? serviceScope)
                                                                                                         ? (a, serviceScope)
                                                                                                         : (null, null))
                                                                                             .Where(x => x.Item1 is not null)
                                                                                             .ToList();

        foreach ((IAssemblySymbol? assembly, ServiceScope? serviceScope) in referencedAssemblies)
        {
            AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyScanConfiguration(assembly!, serviceScope!);
            assemblyCollection.Add(assembly!).WithConfiguration(assemblyScanConfiguration);
        }

        // At this point, if there's nothing in the map then there's nothing to generate. So bail out.
        if (assemblyCollection.IsEmpty)
        {
            return null;
        }

        TypeSymbols allTypesInAllAssemblies = GetAllTypeSymbolsInAllAssemblies(assemblyCollection);

        RunConfiguration runParameter = new(generationConfiguration, allTypesInAllAssemblies);

        return runParameter;
    }

    private AssemblyScanConfiguration BuildAssemblyScanConfiguration(IAssemblySymbol assembly, ServiceScope? defaultServiceScope)
    {
        AssemblyScanConfiguration result = new();

        List<string> excludedNamespaces = assembly.GetExcludedNamespaces();

        if (excludedNamespaces.Count > 0)
        {
            result.ExcludedNamespaces.AddRange(excludedNamespaces);
        }

        result.IncludeMicrosoftNamespaces = assembly.ShouldIncludeMicrosoftNamespaces();
        result.IncludeSystemNamespaces = assembly.ShouldIncludeSystemNamespaces();
        result.DefaultServiceScope = (ServiceScope)defaultServiceScope!;

        var classSuffixAttributes = assembly.GetAttributesOfType<SuffixRegistration>();

        if (classSuffixAttributes.Count > 0)
        {
            List<string> classSuffixes = new();

            foreach (AttributeData attribute in classSuffixAttributes)
            {
                var classSuffix = attribute.GetValueForPropertyOfType<string>(nameof(SuffixRegistration.Suffix));

                if (!string.IsNullOrWhiteSpace(classSuffix))
                {
                    classSuffixes.Add(classSuffix);
                }
            }

            result.ClassSuffixes.AddRange(classSuffixes);
        }

        return result;
    }

    private TypeSymbols GetAllTypeSymbolsInAllAssemblies(Assemblies assemblies)
    {
        TypeSymbols result = new();

        foreach (var assemblyMap in assemblies)
        {
            IAssemblySymbol? assembly = assemblyMap.Key;
            AssemblyScanConfiguration? scanConfiguration = assemblyMap.Value;

            Namespaces namespaces = GetNamespacesFromAssembly(assembly, scanConfiguration);

            if (namespaces.Count == 0)
            {
                continue;
            }

            foreach (INamespaceSymbol? @namespace in namespaces)
            {
                var types = GetTypesFromNamespace(@namespace!, scanConfiguration!);

                foreach (var type in types)
                {
                    result.Add(type);
                }
            }
        }

        return result;
    }

    private Namespaces GetNamespacesFromAssembly(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyScanConfiguration)
    {
        Namespaces namespaces = new();

        INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
        List<string> excludedNamespaces = assemblyScanConfiguration.ExcludedNamespaces;
        var includeMicrosoftNamespaces = assemblyScanConfiguration.IncludeMicrosoftNamespaces;
        var includeSystemNamespaces = assemblyScanConfiguration.IncludeSystemNamespaces;

        foreach (INamespaceSymbol @namespace in globalNamespace.GetNamespaces())
        {
            var namespaceName = @namespace.ToDisplayString();

            if (excludedNamespaces.Contains(namespaceName)
                || (namespaceName.StartsWith("Microsoft") && !includeMicrosoftNamespaces)
                || (namespaceName.StartsWith("System") && !includeSystemNamespaces))
            {
                continue;
            }

            namespaces.Add(@namespace);
        }

        return namespaces;
    }

    private TypeSymbols GetTypesFromNamespace(
        INamespaceSymbol @namespace,
        AssemblyScanConfiguration assemblyScanConfiguration)
    {
        TypeSymbols result = new();

        List<INamedTypeSymbol> concreteTypes = @namespace.GetConcreteTypes();

        if (concreteTypes.Count == 0)
        {
            return result;
        }

        var assemblyHasOneOrMoreClassSuffix = assemblyScanConfiguration.ClassSuffixes.Count > 0;

        foreach (INamedTypeSymbol typeSymbol in concreteTypes)
        {
            if (typeSymbol.HasAttributeOfType<ExcludeRegistration>())
            {
                continue;
            }

            ServiceScope? typeServiceScope = null;

            AttributeData? serviceScopeAttribute = typeSymbol.GetFirstAttributeOfType<UseScope>();

            if (serviceScopeAttribute is not null)
            {
                typeServiceScope = serviceScopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(UseScope.ServiceScope));
            }

            ServiceScope serviceScope = typeServiceScope ?? assemblyScanConfiguration.DefaultServiceScope;

            if (assemblyHasOneOrMoreClassSuffix)
            {
                foreach (var suffix in assemblyScanConfiguration.ClassSuffixes)
                {
                    if (typeSymbol.Name.EndsWith(suffix))
                    {
                        result.Add(new Type
                        {
                            ServiceScope = serviceScope,
                            Symbol = typeSymbol,
                        });
                    }
                }

                continue;
            }

            result.Add(new Type
            {
                ServiceScope = serviceScope,
                Symbol = typeSymbol,
            });
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

        foreach (var type in runConfiguration.TypesToRegister)
        {
            ITypeSymbol? typeSymbol = type.Symbol;
            ServiceScope typeScope = type.ServiceScope;

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