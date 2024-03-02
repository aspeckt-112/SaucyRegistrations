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
using SaucyRegistrations.Generators.Logging;

using Type = SaucyRegistrations.Generators.Models.Type;

namespace SaucyRegistrations.Generators;

/// <summary>
/// The source generator for the Saucy library.
/// </summary>
[Generator]
public class SaucyGenerator : ISourceGenerator
{
    private readonly Logger _logger;
    private readonly GenerationConfigurationBuilder _generationConfigurationBuilder;
    private readonly AssemblyBuilder _assemblyBuilder;
    // private readonly TypeSymbolsBuilder _typeSymbolsBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaucyGenerator" /> class.
    /// </summary>
    public SaucyGenerator()
    {
        _logger = new Logger();
        _generationConfigurationBuilder = new GenerationConfigurationBuilder(_logger);
        _assemblyBuilder = new AssemblyBuilder(_logger);
        // _typeSymbolsBuilder = new TypeSymbolsBuilder(_logger);
    }

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
        _logger.WriteInformation("Starting source generation...");

        Assemblies assembliesToScan = _assemblyBuilder.Build(context.Compilation);

        if (assembliesToScan.Count == 0)
        {
            _logger.WriteWarning("No assemblies found. Exiting source generation. No source will be generated.");

            return;
        }

        Dictionary<ITypeSymbol, ServiceScope> typeSymbols = new();

        (string Namespace, string Class, string Method)? generationConfiguration = null;

        foreach (IAssemblySymbol assemblySymbol in assembliesToScan)
        {
            var assemblyNamespaces = assemblySymbol.GlobalNamespace.GetNamespaces().ToList();

            if (generationConfiguration is null)
            {
                void BuildGenerationConfiguration()
                {
                    foreach (INamespaceSymbol? assemblyNamespace in assemblyNamespaces)
                    {
                        foreach (INamedTypeSymbol? namedTypeSymbol in assemblyNamespace.GetTypeMembers())
                        {
                            foreach (AttributeData? attribute in namedTypeSymbol.GetAttributes())
                            {
                                if (attribute.Is<ServiceCollectionMethod>())
                                {
                                    var methodName = attribute.GetValueForPropertyOfType<string>(nameof(ServiceCollectionMethod.MethodName));
                                    generationConfiguration = (assemblyNamespace.ToDisplayString(), namedTypeSymbol.Name, methodName);

                                    return;
                                }
                            }
                        }
                    }
                }

                BuildGenerationConfiguration();

                // If the generation configuration is still null, we can't continue.
                if (generationConfiguration is null)
                {
                    _logger.WriteWarning("No generation configuration found. Exiting source generation. No source will be generated.");

                    return;
                }

                var flatListOfTypes
                    = (
                        from @namespace in assemblyNamespaces
                        from type in @namespace.GetConcreteTypes()
                        let excludeTypeAttribute = type.GetFirstAttributeOfType<SaucyExclude>()
                        where excludeTypeAttribute is null
                        select type).ToList();

                var explicitlyRegisteredTypes =
                    from type in flatListOfTypes
                    let addTypeAttribute = type.GetFirstAttributeOfType<SaucyAddType>()
                    where addTypeAttribute is not null
                    let scopeAttribute = type.GetFirstAttributeOfType<SaucyScope>()
                    where scopeAttribute is not null
                    select new { Type = type, ServiceScope = scopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(SaucyScope.ServiceScope)) };

                foreach (var registeredType in explicitlyRegisteredTypes)
                {
                    typeSymbols.Add(registeredType.Type, registeredType.ServiceScope);
                }

                // Now there's a list of types that have been explicitly registered, we can remove them from the list of types
                // that we're going to scan for.
                flatListOfTypes.RemoveAll(x => typeSymbols.ContainsKey(x));

                List<(string, ServiceScope)> namespacesToIncludeAllTypesWithin = new();

                // If the namespace is any of the namespaces in the assembly
                // that we want to include everything in, do that
                List<AttributeData> addNamespaceAttributes = assemblySymbol.GetAttributesOfType<SaucyAddNamespace>();

                foreach (AttributeData attribute in addNamespaceAttributes)
                {
                    var namespaceToAdd = attribute.GetValueForPropertyOfType<string>(nameof(SaucyAddNamespace.Namespace));
                    ServiceScope scope = attribute.GetValueForPropertyOfType<ServiceScope>(nameof(SaucyAddNamespace.Scope));
                    namespacesToIncludeAllTypesWithin.Add((namespaceToAdd, scope));
                }

                foreach ((var namespaceToAdd, ServiceScope scope) in namespacesToIncludeAllTypesWithin)
                {
                    var includedNamespaces = assemblyNamespaces.Where(x => x.ToDisplayString().EndsWith(namespaceToAdd)).ToList();

                    foreach (INamespaceSymbol? @namespace in includedNamespaces)
                    {
                        List<INamedTypeSymbol> types = @namespace.GetConcreteTypes();

                        foreach (INamedTypeSymbol? type in types)
                        {
                            // Check to see if the type has a custom scope.
                            AttributeData? scopeAttribute = type.GetFirstAttributeOfType<SaucyScope>();

                            if (scopeAttribute is not null)
                            {
                                ServiceScope customScope = scopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(SaucyScope.ServiceScope));
                                typeSymbols.Add(type, customScope);

                                continue;
                            }

                            // Type doesn't have a custom scope. Use the scope from the namespace.
                            typeSymbols.Add(type, scope);
                        }
                    }
                }
            }
        }

        string registrationCode = GenerateRegistrationCode(generationConfiguration!.Value, typeSymbols);

        context.AddSource("SaucyRegistrations_Generated.cs", registrationCode);
    }

    private string GenerateRegistrationCode((string Namespace, string Class, string Method) generationConfiguration, Dictionary<ITypeSymbol, ServiceScope> typeSymbols)
    {
        StringBuilder sourceBuilder = new();

        sourceBuilder.Append(
            $@"//<auto-generated by Saucy on {DateTime.Now} />
using Microsoft.Extensions.DependencyInjection;

namespace {generationConfiguration.Namespace}
{{
	public static partial class {generationConfiguration.Class}
	{{
		public static void {generationConfiguration.Method}(IServiceCollection serviceCollection)
		{{"
        );

        sourceBuilder.AppendLine();

        Dictionary<ServiceScope, string> serviceScopeToMethodNameMap = new()
        {
            { ServiceScope.Singleton, "serviceCollection.AddSingleton" },
            { ServiceScope.Scoped, "serviceCollection.AddScoped" },
            { ServiceScope.Transient, "serviceCollection.AddTransient" },
        };

        foreach (var type in typeSymbols)
        {
            ITypeSymbol typeSymbol = type.Key;
            ServiceScope typeScope = type.Value;

            var fullyQualifiedTypeName = typeSymbol.ToDisplayString();

            INamedTypeSymbol? classBaseType = typeSymbol.BaseType;

            var classHasBaseType = classBaseType is not null;

            if (classHasBaseType && typeSymbol.BaseType!.IsAbstract)
            {
                var fullyQualifiedBaseTypeName = typeSymbol.BaseType.ToDisplayString();

                sourceBuilder.AppendLine($@"            {serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedBaseTypeName}, {fullyQualifiedTypeName}>();");
            }

            var classHasInterfaces = typeSymbol.Interfaces.Length > 0;

            switch (classHasInterfaces)
            {
                case true:
                    {
                        foreach (INamedTypeSymbol @interface in typeSymbol.Interfaces)
                        {
                            var fullyQualifiedInterfaceName = @interface.ToDisplayString();

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