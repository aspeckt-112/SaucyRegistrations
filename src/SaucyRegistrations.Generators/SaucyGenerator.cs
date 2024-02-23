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

        Namespaces namespaces = new();

        Dictionary<ITypeSymbol, ServiceScope> typeSymbols = new();

        foreach (IAssemblySymbol assemblySymbol in assembliesToScan)
        {
            var assemblyNamespaces = assemblySymbol.GlobalNamespace.GetNamespaces().ToList();

            var flatListOfTypes = assemblyNamespaces.SelectMany(x => x.GetConcreteTypes()).ToList();

            var explicitlyRegisteredTypes =
                from type in flatListOfTypes
                let addTypeAttribute = type.GetFirstAttributeOfType<SaucyAddType>()
                where addTypeAttribute is not null
                    select new
                    {
                        Type = type,
                        ServiceScope = addTypeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(SaucyAddType.ServiceScope)),
                    };

            foreach (var registeredType in explicitlyRegisteredTypes)
            {
                typeSymbols.Add(registeredType.Type, registeredType.ServiceScope);
            }

            // Now there's a list of types that have been explicitly registered, we can remove them from the list of types
            // that we're going to scan for.
            flatListOfTypes.RemoveAll(x => typeSymbols.ContainsKey(x));

            List<(string, ServiceScope)> namespacesToIncludeAll = new();

            // If the namespace is any of the namespaces in the assembly
            // that we want to include everything in, do that
            var addNamespaceAttributes = assemblySymbol.GetAttributesOfType<SaucyAddNamespace>();

            foreach (AttributeData attribute in addNamespaceAttributes)
            {
                var namespaceToAdd = attribute.GetValueForPropertyOfType<string>(nameof(SaucyAddNamespace.Namespace));
                var scope = attribute.GetValueForPropertyOfType<ServiceScope>(nameof(SaucyAddNamespace.Scope));
                namespacesToIncludeAll.Add((namespaceToAdd, scope));
            }

            foreach (var (namespaceToAdd, scope) in namespacesToIncludeAll)
            {
                var things = assemblyNamespaces.Where(x => x.ToDisplayString().EndsWith(namespaceToAdd)).ToList();

                foreach (var thing in things)
                {
                    var types = thing.GetConcreteTypes();

                    foreach (var type in types)
                    {
                        typeSymbols.Add(type, scope);
                    }
                }
            }

            namespaces.AddRange(assemblyNamespaces);
        }

        throw new NotImplementedException();


        // GenerationConfiguration? generationConfiguration = _generationConfigurationBuilder.Build(context.Compilation);
        //
        // if (generationConfiguration is null)
        // {
        //     _logger.WriteWarning("No generation configuration found. Exiting source generation. No source will be generated.");
        //     _logger.WriteWarning($"Ensure you've added the [{nameof(ServiceCollectionMethod)}] attribute to a class in your project.");
        //
        //     return;
        // }
        //
        // TypeSymbols typeSymbols = _typeSymbolsBuilder.Build(assembliesToScan);
        //
        // if (typeSymbols.Count == 0)
        // {
        //     _logger.WriteWarning("No type symbols found. Exiting source generation. No source will be generated.");
        //     _logger.WriteWarning("Ensure that you've configured the Saucy library correctly.");
        //
        //     return;
        // }
        //
        // var source = GenerateRegistrationCode(generationConfiguration, typeSymbols, context.Compilation.ObjectType);
        //
        // context.AddSource($"{generationConfiguration.Class}.Generated.cs", source);
    }

    private string GenerateRegistrationCode(GenerationConfiguration generationConfiguration, TypeSymbols typeSymbols, INamedTypeSymbol objectSymbol)
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

        foreach (Type type in typeSymbols)
        {
            ITypeSymbol typeSymbol = type.Symbol;
            ServiceScope typeScope = type.ServiceScope;

            var fullyQualifiedTypeName = typeSymbol.ToDisplayString();

            INamedTypeSymbol? classBaseType = typeSymbol.BaseType;

            var classHasBaseType = classBaseType is not null && !ReferenceEquals(classBaseType, objectSymbol);

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