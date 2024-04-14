using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SaucyRegistrations.Generators.Comparers;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Infrastructure;
using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.SourceConstants.Attributes;
using SaucyRegistrations.Generators.SourceConstants.Enums;

namespace SaucyRegistrations.Generators;

/// <summary>
/// The Saucy generator.
/// </summary>
[Generator]
public sealed class SaucyGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterAttributesAndEnums(context);

        IncrementalValueProvider<string> assemblyNameProvider = GetAssemblyNameProvider(context);
        IncrementalValueProvider<ImmutableArray<ServiceDefinition>> assemblyAttributesProvider = GetNamespacesToIncludeProvider(context);
        IncrementalValuesProvider<ServiceDefinition> serviceDefinitionProvider = CreateServiceDefinitionProvider(context);

        // Horrible tuple. Make use of aliases in the future?
        IncrementalValueProvider<((ImmutableArray<ServiceDefinition> Services, string AssemblyName) ExplicitlyRegisteredServices, ImmutableArray<ServiceDefinition>
            NamespaceRegisteredServices)> servicesProvider = CombineProviders(serviceDefinitionProvider, assemblyNameProvider, assemblyAttributesProvider);

        RegisterSourceOutput(context, servicesProvider);
    }

    private void RegisterAttributesAndEnums(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            ctx =>
            {
                AddSaucyAttributes(ctx);
                AddSaucyEnums(ctx);
            }
        );
    }

    private void AddSaucyAttributes(IncrementalGeneratorPostInitializationContext ctx)
    {
        ctx.AddSource("Saucy.Attributes.SaucyInclude.g.cs", SourceText.From(SaucyInclude.SaucyIncludeAttributeDefinition, Encoding.UTF8));

        ctx.AddSource(
            "Saucy.Attributes.SaucyIncludeNamespaceWithSuffix.g.cs",
            SourceText.From(SaucyIncludeNamespaceWithSuffix.SaucyIncludeNamespaceWithSuffixAttributeDefinition, Encoding.UTF8)
        );

        ctx.AddSource(
            "Saucy.Attributes.SaucyOnlyRegisterInterface.g.cs", SourceText.From(SaucyRegisterAbstractClass.SaucyRegisterAbstractClassAttributeDefinition, Encoding.UTF8)
        );
    }

    private void AddSaucyEnums(IncrementalGeneratorPostInitializationContext ctx)
    {
        ctx.AddSource("Saucy.Enums.ServiceScope.g.cs", SourceText.From(ServiceScope.ServiceScopeEnumDefinition, Encoding.UTF8));
    }

    private IncrementalValueProvider<string> GetAssemblyNameProvider(IncrementalGeneratorInitializationContext context)
    {
        return context.CompilationProvider.GetAssemblyName();
    }

    private IncrementalValueProvider<ImmutableArray<ServiceDefinition>> GetNamespacesToIncludeProvider(IncrementalGeneratorInitializationContext context)
    {
        return context.CompilationProvider.GetNamespacesToInclude();
    }

    private IncrementalValuesProvider<ServiceDefinition> CreateServiceDefinitionProvider(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider.CreateSyntaxProvider(NodeIsClassDeclarationWithSaucyAttributes, GetServiceDetails);
    }

    private bool NodeIsClassDeclarationWithSaucyAttributes(SyntaxNode node, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return node is ClassDeclarationSyntax cds && cds.AttributeLists.SelectMany(x => x.Attributes).Any(y => y.Name.ToString() == nameof(SaucyInclude));
    }

    private ServiceDefinition GetServiceDetails(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var symbol = (context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol) !;

        AttributeData saucyIncludeAttribute = symbol.GetAttributes().First(x => x.AttributeClass?.Name == nameof(SaucyInclude));
        var serviceScope = (int)saucyIncludeAttribute.ConstructorArguments[0].Value!;

        return new ServiceDefinition(symbol.GetFullyQualifiedName(), serviceScope, symbol.GetContractDefinitions());
    }

    private IncrementalValueProvider<((ImmutableArray<ServiceDefinition> Services, string AssemblyName) ExplicitlyRegisteredServices, ImmutableArray<ServiceDefinition>
        NamespaceRegisteredServices)> CombineProviders(
        IncrementalValuesProvider<ServiceDefinition> serviceDefinitionProvider,
        IncrementalValueProvider<string> assemblyNameProvider,
        IncrementalValueProvider<ImmutableArray<ServiceDefinition>> assemblyAttributesProvider
    )
    {
        return serviceDefinitionProvider.Collect().Combine(assemblyNameProvider).Combine(assemblyAttributesProvider);
    }

    private void RegisterSourceOutput(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<((ImmutableArray<ServiceDefinition> Services, string AssemblyName) ExplicitlyRegisteredServices, ImmutableArray<ServiceDefinition>
            NamespaceRegisteredServices)> servicesProvider
    )
    {
        context.RegisterSourceOutput(
            servicesProvider, (ctx, servicesPair) =>
            {
                ImmutableArray<ServiceDefinition> servicesFromNamespace = !servicesPair.NamespaceRegisteredServices.IsDefault
                    ? servicesPair.NamespaceRegisteredServices
                    : ImmutableArray<ServiceDefinition>.Empty;

                ImmutableArray<ServiceDefinition> servicesExplicitlyRegistered = !servicesPair.ExplicitlyRegisteredServices.Services.IsDefault
                    ? servicesPair.ExplicitlyRegisteredServices.Services
                    : ImmutableArray<ServiceDefinition>.Empty;

                var assemblyName = servicesPair.ExplicitlyRegisteredServices.AssemblyName;

                var servicesToRegister = new HashSet<ServiceDefinition>(servicesExplicitlyRegistered, new ServiceDefinitionComparer());

                foreach (ServiceDefinition service in servicesFromNamespace)
                {
                    servicesToRegister.Add(service);
                }

                Generate(ctx, assemblyName, servicesToRegister);
            }
        );
    }

    private void Generate(SourceProductionContext context, string assemblyName, HashSet<ServiceDefinition> servicesToRegister)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var writer = new SourceWriter();

        Dictionary<int, string> serviceScopeEnumValues = new()
        {
            { ServiceScope.SingletonScopeValue, "services.AddSingleton" },
            { ServiceScope.TransientScopeValue, "services.AddTransient" },
            { ServiceScope.ScopedScopeValue, "services.AddScoped" }
        };

        var assemblyNameWithoutPeriods = assemblyName.Replace(".", string.Empty);
        var className = $"{assemblyNameWithoutPeriods}ServiceCollectionExtensions";

        writer.AppendLine($"// <auto-generated by Saucy on {DateTime.Now} />")
              .AppendLine("using Microsoft.Extensions.DependencyInjection;")
              .AppendLine()
              .AppendLine($"namespace {assemblyName}.ServiceCollectionExtensions;")
              .AppendLine()
              .AppendLine($"public static class {className}")
              .AppendLine("{")
              .Indent()
              .AppendLine($"public static IServiceCollection Add{assemblyNameWithoutPeriods}Services(this IServiceCollection services)")
              .AppendLine("{")
              .Indent();

        var serviceCount = servicesToRegister.Count;

        if (serviceCount == 0)
        {
            writer.AppendLine("return services;");
        }
        else
        {
            foreach (ServiceDefinition serviceDefinition in servicesToRegister.OrderBy(x => x.FullyQualifiedClassName))
            {
                var serviceScopeValue = (int)serviceDefinition.ServiceScope!;
                var serviceScope = serviceScopeEnumValues[serviceScopeValue];

                if (serviceDefinition.HasContracts)
                {
                    foreach (var contractDefinition in serviceDefinition.ContractDefinitions!)
                    {
                        var name = contractDefinition.FullyQualifiedTypeName;
                        if (contractDefinition.IsGeneric)
                        {
                            var genericTypes = string.Join(",", contractDefinition.FullyQualifiedGenericTypeNames!);
                            writer.AppendLine($"{serviceScope}<{name}<{genericTypes}>, {serviceDefinition.FullyQualifiedClassName}>();");
                        }
                        else
                        {
                            writer.AppendLine($"{serviceScope}<{name}, {serviceDefinition.FullyQualifiedClassName}>();");
                        }
                    }
                }
                else
                {
                    writer.AppendLine($"{serviceScope}<{serviceDefinition.FullyQualifiedClassName}>();");
                }
            }

            writer.AppendLine("return services;");
        }

        writer.UnIndent().AppendLine('}').UnIndent().AppendLine('}');
        context.AddSource($"{assemblyNameWithoutPeriods}.{className}.g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
    }
}