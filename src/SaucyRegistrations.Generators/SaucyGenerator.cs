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
using SaucyRegistrations.Generators.Models.Contracts;
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
            AssemblyAttributesProvider assemblyAttributesProvider =
                context.CompilationProvider.GetNamespacesToInclude();

            IncrementalValuesProvider<ServiceDefinition> serviceDefinitionProvider =
                context.SyntaxProvider.CreateSyntaxProvider(NodeIsClassDeclarationWithSaucyAttributes,
                    GetServiceDetails);

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

    private void Generate(SourceProductionContext context, string assemblyName,
        HashSet<ServiceDefinition> servicesToRegister)
    {
        CancellationToken cancellationToken = context.CancellationToken;
        cancellationToken.ThrowIfCancellationRequested();

        var sourceWriter = new SourceWriter();
        var assemblyNameWithoutPeriods = assemblyName.Replace(".", string.Empty);
        var className = $"{assemblyNameWithoutPeriods}ServiceCollectionExtensions";

        AppendHeader(sourceWriter, assemblyName, className);

        foreach (ServiceDefinition? serviceDefinition in servicesToRegister.OrderBy(x => x.FullyQualifiedClassName))
        {
            cancellationToken.ThrowIfCancellationRequested();
            AppendServiceRegistration(sourceWriter, serviceDefinition, cancellationToken);
        }

        AppendFooter(sourceWriter);

        var hintName = $"{assemblyNameWithoutPeriods}.{className}.g.cs";
        context.AddSource(hintName, SourceText.From(sourceWriter.ToString(), Encoding.UTF8));
    }

    private void AppendHeader(
        SourceWriter sourceWriter,
        string assemblyName,
        string className)
    {
        sourceWriter.AppendLine("// <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />")
            .AppendLine("using Microsoft.Extensions.DependencyInjection;")
            .AppendLine()
            .AppendLine($"namespace {assemblyName}.ServiceCollectionExtensions;")
            .AppendLine()
            .AppendLine($"public static class {className}")
            .AppendLine("{")
            .Indent()
            .AppendLine(
                $"public static IServiceCollection Add{assemblyName.Replace(".", string.Empty)}Services(this IServiceCollection services)")
            .AppendLine("{")
            .Indent();
    }

    private void AppendServiceRegistration(
        SourceWriter writer,
        ServiceDefinition serviceDefinition,
        CancellationToken cancellationToken)
    {
        var serviceScope = GetServiceScopeString(serviceDefinition);

        if (serviceDefinition.HasContracts)
        {
            foreach (ContractDefinition? contractDefinition in serviceDefinition.ContractDefinitions!)
            {
                cancellationToken.ThrowIfCancellationRequested();
                AppendContractRegistration(writer, serviceDefinition, contractDefinition, serviceScope);
            }
        }
        else
        {
            AppendSimpleRegistration(writer, serviceDefinition, serviceScope);
        }
    }

    private string GetServiceScopeString(ServiceDefinition serviceDefinition)
    {
        var serviceScopes = new Dictionary<int, string>
        {
            { ServiceScope.SingletonScopeValue, "Singleton" },
            { ServiceScope.TransientScopeValue, "Transient" },
            { ServiceScope.ScopedScopeValue, "Scoped" }
        };

        var scope = serviceScopes[(int)serviceDefinition.ServiceScope!];

        var sb = new StringBuilder();
        sb.Append("services.");
        sb.Append(serviceDefinition.IsKeyed ? $"AddKeyed{scope}" : $"Add{scope}");
        return sb.ToString();
    }

    private void AppendContractRegistration(
        SourceWriter writer,
        ServiceDefinition serviceDefinition,
        ContractDefinition contractDefinition,
        string serviceScope)
    {
        var key = serviceDefinition.IsKeyed ? $"\"{serviceDefinition.Key}\"" : string.Empty;
        var registrationString = ConstructRegistrationString(serviceDefinition, contractDefinition, serviceScope, key);
        writer.AppendLine(registrationString);
    }

    private string ConstructRegistrationString(
        ServiceDefinition serviceDefinition,
        ContractDefinition contractDefinition,
        string serviceScope,
        string key)
    {
        if (contractDefinition is KnownNamedTypeSymbolGenericContractDefinition closedGenericContractDefinition)
        {
            var genericTypes = string.Join(",", closedGenericContractDefinition.GenericTypeNames!);
            return
                $"{serviceScope}<{contractDefinition.TypeName}<{genericTypes}>, {serviceDefinition.FullyQualifiedClassName}>({key});";
        }

        if (contractDefinition is OpenGenericContractDefinition openContractDefinition)
        {
            var contractArityString = openContractDefinition.Arity.GetArityString();

            StringBuilder serviceDefinitionArityBuilder = new();

            if (serviceDefinition is GenericServiceDefinition genericServiceDefinition)
            {
                serviceDefinitionArityBuilder.Append(genericServiceDefinition.Arity.GetArityString());
            }

            var keyText = string.IsNullOrWhiteSpace(key) ? ", " : $", {key}, ";
            return
                $"{serviceScope}(typeof({contractDefinition.TypeName}{contractArityString}){keyText}typeof({serviceDefinition.FullyQualifiedClassName}{serviceDefinitionArityBuilder}));";
        }

        return
            $"{serviceScope}<{contractDefinition.TypeName}, {serviceDefinition.FullyQualifiedClassName}>({key});";
    }

    private void AppendSimpleRegistration(
        SourceWriter writer,
        ServiceDefinition serviceDefinition,
        string serviceScope)
    {
        var key = serviceDefinition.IsKeyed ? $", \"{serviceDefinition.Key}\"" : string.Empty;

        StringBuilder serviceDefinitionArityBuilder = new();

        if (serviceDefinition is GenericServiceDefinition genericServiceDefinition)
        {
            serviceDefinitionArityBuilder.Append(genericServiceDefinition.Arity.GetArityString());
        }

        var registrationString = serviceDefinition is GenericServiceDefinition
            ? $"{serviceScope}(typeof({serviceDefinition.FullyQualifiedClassName}{serviceDefinitionArityBuilder}){key});"
            : $"{serviceScope}<{serviceDefinition.FullyQualifiedClassName}>({key.TrimStart(',').TrimStart(' ')});";

        writer.AppendLine(registrationString);
    }

    private void AppendFooter(SourceWriter sourceWriter)
    {
        sourceWriter.AppendLine("return services;")
            .UnIndent()
            .AppendLine("}")
            .UnIndent()
            .Append('}');
    }
}