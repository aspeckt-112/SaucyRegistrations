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
using SaucyRegistrations.Generators.Factories;
using SaucyRegistrations.Generators.Infrastructure;
using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.Models.Contracts;
using SaucyRegistrations.Generators.SourceConstants.Attributes;
using SaucyRegistrations.Generators.SourceConstants.Enums;

namespace SaucyRegistrations.Generators;

/// <summary>
/// The Saucy generator.
/// </summary>
[Generator]
public sealed partial class SaucyGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            context.RegisterPostInitializationOutput(
                ctx =>
                {
                    AddSaucyAttributes(ctx);
                    AddSaucyEnums(ctx);
                }
            );

            AssemblyName assemblyName = context
                .CompilationProvider
                .GetAssemblyName();

            ServiceDefinitionsFromNamespace serviceDefinitionsFromNamespace = context
                .CompilationProvider
                .GetServiceDefinitionsFromIncludedNamespaces();

            IncrementalValuesProvider<ServiceDefinition> serviceDefinitionProvider =
                context.SyntaxProvider.CreateSyntaxProvider(
                    NodeIsClassDeclarationWithSaucyAttributes,
                    GetServiceDetails);

            ServicesProvider servicesProvider = serviceDefinitionProvider
                .Collect()
                .Combine(assemblyName)
                .Combine(serviceDefinitionsFromNamespace);

            RegisterSourceOutput(context, servicesProvider);
        }
        catch (OperationCanceledException)
        {
            // Intentionally swallow the exception - the operation was cancelled.
        }
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
                CancellationToken cancellationToken = ctx.CancellationToken;

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
        CancellationToken cancellationToken = context.CancellationToken;
        cancellationToken.ThrowIfCancellationRequested();

        var sourceWriter = new SourceWriter();
        var assemblyNameWithoutPeriods = assemblyName.Replace(".", string.Empty);
        var className = $"{assemblyNameWithoutPeriods}ServiceCollectionExtensions";

        sourceWriter.AppendHeader(assemblyName, className);

        foreach (ServiceDefinition? serviceDefinition in servicesToRegister.OrderBy(x => x.FullyQualifiedClassName))
        {
            cancellationToken.ThrowIfCancellationRequested();
            AppendServiceRegistration(sourceWriter, serviceDefinition, cancellationToken);
        }

        sourceWriter.AppendFooter();

        var hintName = $"{assemblyNameWithoutPeriods}.{className}.g.cs";
        context.AddSource(hintName, SourceText.From(sourceWriter.ToString(), Encoding.UTF8));
    }

    private void AppendServiceRegistration(SourceWriter writer, ServiceDefinition serviceDefinition, CancellationToken cancellationToken)
    {
        var methodName = GetServiceScopeMethodName(serviceDefinition);

        if (serviceDefinition.HasContracts)
        {
            foreach (var contractDefinition in serviceDefinition.ContractDefinitions!)
            {
                cancellationToken.ThrowIfCancellationRequested();
                AppendRegistration(writer, serviceDefinition, contractDefinition, methodName);
            }
        }
        else
        {
            AppendRegistration(writer, serviceDefinition, null, methodName);
        }
    }

    private string GetServiceScopeMethodName(ServiceDefinition serviceDefinition)
    {
        var methodName = serviceDefinition.ServiceScope switch
        {
            ServiceScope.SingletonScopeValue => "services.Add{0}Singleton",
            ServiceScope.TransientScopeValue => "services.Add{0}Transient",
            ServiceScope.ScopedScopeValue => "services.Add{0}Scoped",
            _ => throw new ArgumentOutOfRangeException(nameof(serviceDefinition.ServiceScope), $"Unsupported service scope: {serviceDefinition.ServiceScope}.")
        };

        return string.Format(methodName, serviceDefinition.IsKeyed ? "Keyed" : string.Empty);
    }

    private void AppendRegistration(SourceWriter writer, ServiceDefinition serviceDefinition, ContractDefinition? contractDefinition, string methodName)
    {
        var key = serviceDefinition.IsKeyed ? $"\"{serviceDefinition.Key}\"" : string.Empty;

        var registrationString = contractDefinition != null
            ? ConstructContractRegistrationString(serviceDefinition, contractDefinition, methodName, key)
            : ConstructSimpleRegistrationString(serviceDefinition, methodName, key);

        writer.AppendLine(registrationString);
    }

    private string ConstructContractRegistrationString(
        ServiceDefinition serviceDefinition,
        ContractDefinition contractDefinition,
        string methodName,
        string key)
    {
        switch (contractDefinition)
        {
            case KnownNamedTypeSymbolGenericContractDefinition closedGenericContractDefinition:
                {
                    var genericTypes = string.Join(",", closedGenericContractDefinition.GenericTypeNames);
                    return $"{methodName}<{contractDefinition.TypeName}<{genericTypes}>, {serviceDefinition.FullyQualifiedClassName}>({key});";
                }

            case OpenGenericContractDefinition openContractDefinition:
                {
                    var contractArityString = openContractDefinition.Arity.GetArityString();

                    StringBuilder serviceDefinitionArityBuilder = new();

                    if (serviceDefinition is GenericServiceDefinition genericServiceDefinition)
                    {
                        serviceDefinitionArityBuilder.Append(genericServiceDefinition.Arity.GetArityString());
                    }

                    return $"{methodName}(typeof({contractDefinition.TypeName}{contractArityString}){(string.IsNullOrWhiteSpace(key) ? ", " : $", {key}, ")}typeof({serviceDefinition.FullyQualifiedClassName}{serviceDefinitionArityBuilder}));";
                }

            case HostedServiceContractDefinition hostedServiceContractDefinition:
                return $"services.AddHostedService<{hostedServiceContractDefinition.TypeName}>({key});";

            default:
                return $"{methodName}<{contractDefinition.TypeName}, {serviceDefinition.FullyQualifiedClassName}>({key});";
        }
    }

    private string ConstructSimpleRegistrationString(ServiceDefinition serviceDefinition, string methodName, string key)
    {
        var keyParameter = string.IsNullOrEmpty(key) ? string.Empty : $", {key}";

        return serviceDefinition is GenericServiceDefinition genericServiceDefinition
            ? $"{methodName}(typeof({serviceDefinition.FullyQualifiedClassName}{genericServiceDefinition.Arity.GetArityString()}){keyParameter});"
            : $"{methodName}<{serviceDefinition.FullyQualifiedClassName}>({key.TrimStart(',').TrimStart(' ')});";
    }
}