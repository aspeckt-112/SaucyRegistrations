using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SaucyRegistrations.Generators.Builders;
using SaucyRegistrations.Generators.Comparers;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Factories;
using SaucyRegistrations.Generators.Infrastructure;
using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.SourceConstants.Attributes;
using SaucyRegistrations.Generators.SourceConstants.Enums;

// ReSharper disable UnusedVariable -- Used for debugging.
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
        context.RegisterPostInitializationOutput(
            ctx =>
            {
                AddSaucyAttributes(ctx);
                AddSaucyEnums(ctx);
            }
        );

        try
        {
            AssemblyNameProvider assemblyNameProvider = context.CompilationProvider.GetAssemblyName();
            AssemblyAttributesProvider assemblyAttributesProvider = context.CompilationProvider.GetNamespacesToInclude();

            IncrementalValuesProvider<ServiceDefinition> serviceDefinitionProvider =
                context.SyntaxProvider.CreateSyntaxProvider(NodeIsClassDeclarationWithSaucyAttributes, GetServiceDetails);

            ServicesProvider servicesProvider = serviceDefinitionProvider
                .Collect()
                .Combine(assemblyNameProvider)
                .Combine(assemblyAttributesProvider);

            RegisterSourceOutput(context, servicesProvider);
        }
        catch (OperationCanceledException)
        {
            // Do nothing.
        }
    }

    private void AddSaucyAttributes(IncrementalGeneratorPostInitializationContext ctx)
    {
        AttributeDefinitionBuilder allAttributes = new AttributeDefinitionBuilder()
            .AppendAttributeDefinition(SaucyInclude.SaucyIncludeAttributeDefinition)
            .AppendAttributeDefinition(SaucyIncludeNamespace.SaucyIncludeNamespaceWithSuffixAttributeDefinition)
            .AppendAttributeDefinition(SaucyRegisterAbstractClass.SaucyRegisterAbstractClassAttributeDefinition)
            .AppendAttributeDefinition(SaucyDoNotRegisterWithInterface.SaucyDoNotRegisterWithInterfaceDefinition)
            .AppendAttributeDefinition(SaucyExclude.SaucyExcludeAttributeDefinition)
            .AppendAttributeDefinition(SaucyKeyedService.SaucyKeyedServiceDefinition);

        ctx.AddSource("Saucy.Attributes.g.cs", SourceText.From(allAttributes.ToString(), Encoding.UTF8));
    }

    private void AddSaucyEnums(IncrementalGeneratorPostInitializationContext ctx)
    {
        ctx.AddSource("Saucy.Enums.g.cs", SourceText.From(ServiceScope.ServiceScopeEnumDefinition, Encoding.UTF8));
    }

    private bool NodeIsClassDeclarationWithSaucyAttributes(SyntaxNode node, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (node)
        {
            case ClassDeclarationSyntax cds:
                {
                    AttributeSyntax[] attributes = cds
                        .AttributeLists.SelectMany(x => x.Attributes)
                        .ToArray();

                    return attributes.Any(y => y.Name.ToString() == nameof(SaucyInclude)) &&
                           attributes.All(y => y.Name.ToString() != nameof(SaucyExclude));
                }

            default:
                return false;
        }
    }

    private ServiceDefinition GetServiceDetails(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var namedTypeSymbol = (context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol) !;

        AttributeData saucyIncludeAttribute =
            namedTypeSymbol.GetAttributes().First(x => x.AttributeClass?.Name == nameof(SaucyInclude));

        var serviceScope = (int)saucyIncludeAttribute.ConstructorArguments[0].Value!;

        return ServiceDefinitionFactory.CreateServiceDefinition(namedTypeSymbol, serviceScope);
    }

    private void RegisterSourceOutput(
        IncrementalGeneratorInitializationContext context,
        ServicesProvider servicesProvider
    )
    {
        context.RegisterSourceOutput(
            servicesProvider, (ctx, servicesPair) =>
            {
                var cancellationToken = ctx.CancellationToken;

                cancellationToken.ThrowIfCancellationRequested();

                ImmutableArray<ServiceDefinition> servicesFromNamespace =
                    !servicesPair.NamespaceRegisteredServices.IsDefault
                        ? servicesPair.NamespaceRegisteredServices
                        : ImmutableArray<ServiceDefinition>.Empty;

                ImmutableArray<ServiceDefinition> servicesExplicitlyRegistered =
                    !servicesPair.ExplicitlyRegisteredServices.Services.IsDefault
                        ? servicesPair.ExplicitlyRegisteredServices.Services
                        : ImmutableArray<ServiceDefinition>.Empty;

                var assemblyName = servicesPair.ExplicitlyRegisteredServices.AssemblyName;

                var servicesToRegister =
                    new HashSet<ServiceDefinition>(servicesExplicitlyRegistered, new ServiceDefinitionComparer());

                foreach (ServiceDefinition service in servicesFromNamespace)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    servicesToRegister.Add(service);
                }

                Generate(ctx, assemblyName, servicesToRegister);
            }
        );
    }

    private void Generate(
        SourceProductionContext context,
        string assemblyName,
        HashSet<ServiceDefinition> servicesToRegister)
    {
        var cancellationToken = context.CancellationToken;

        cancellationToken.ThrowIfCancellationRequested();

        var writer = new SourceWriter();

        Dictionary<int, string> serviceScopeEnumValues = new()
        {
            { ServiceScope.SingletonScopeValue, "services.Add{0}Singleton" },
            { ServiceScope.TransientScopeValue, "services.Add{0}Transient" },
            { ServiceScope.ScopedScopeValue, "services.Add{0}Scoped" }
        };

        var assemblyNameWithoutPeriods = assemblyName.Replace(".", string.Empty);
        var className = $"{assemblyNameWithoutPeriods}ServiceCollectionExtensions";

        writer.AppendLine($"// <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />")
            .AppendLine("using Microsoft.Extensions.DependencyInjection;")
            .AppendLine()
            .AppendLine($"namespace {assemblyName}.ServiceCollectionExtensions;")
            .AppendLine()
            .AppendLine($"public static class {className}")
            .AppendLine("{")
            .Indent()
            .AppendLine(
                $"public static IServiceCollection Add{assemblyNameWithoutPeriods}Services(this IServiceCollection services)")
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
                cancellationToken.ThrowIfCancellationRequested();

                var isKeyedService = serviceDefinition.IsKeyed;
                var serviceScopeValue = (int)serviceDefinition.ServiceScope!;
                var serviceScope = string.Format(serviceScopeEnumValues[serviceScopeValue], isKeyedService ? "Keyed" : string.Empty);
                var key = isKeyedService ? $"\"{serviceDefinition.Key}\"" : string.Empty;
                // var key = isKeyedService ? string.Empty : string.Empty;

                if (serviceDefinition.HasContracts)
                {
                    foreach (ContractDefinition? contractDefinition in serviceDefinition.ContractDefinitions!)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var name = contractDefinition.FullyQualifiedTypeName;
                        if (contractDefinition.IsGeneric)
                        {
                            var genericTypes = string.Join(",", contractDefinition.FullyQualifiedGenericTypeNames!);
                            writer.AppendLine(
                                $"{serviceScope}<{name}<{genericTypes}>, {serviceDefinition.FullyQualifiedClassName}>({key});");
                        }
                        else
                        {
                            writer.AppendLine(
                                $"{serviceScope}<{name}, {serviceDefinition.FullyQualifiedClassName}>({key});");
                        }
                    }
                }
                else
                {
                    writer.AppendLine($"{serviceScope}<{serviceDefinition.FullyQualifiedClassName}>({key});");
                }
            }

            writer.AppendLine("return services;");
        }

        writer.UnIndent().AppendLine('}').UnIndent().Append('}');

#if DEBUG   // Easy way to see the generated code when debugging.
        var generatedCode = writer.ToString();
#endif

        var hintName = $"{assemblyNameWithoutPeriods}.{className}.g.cs";

        context.AddSource(
            hintName,
            SourceText.From(writer.ToString(), Encoding.UTF8));
    }
}