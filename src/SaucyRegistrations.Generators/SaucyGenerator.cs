﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SaucyRegistrations.Generators.CodeConstants;
using SaucyRegistrations.Generators.Comparers;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Infrastructure;
using SaucyRegistrations.Generators.Models;

namespace SaucyRegistrations.Generators
{
    /// <summary>
    /// The source generator for the Saucy library.
    /// </summary>
    [Generator]
    public sealed class SaucyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(AddSource);

            IncrementalValueProvider<string> assemblyName = context.CompilationProvider.GetAssemblyName();

            IncrementalValueProvider<ImmutableArray<ServiceDefinition>> assemblyAttributes = context.CompilationProvider.GetNamespacesToInclude();

            IncrementalValuesProvider<ServiceDefinition> syntax = context.SyntaxProvider.CreateSyntaxProvider(NodeIsClassDeclarationWithSaucyAttributes, GetServiceDetails);

            IncrementalValueProvider<((ImmutableArray<ServiceDefinition> Left, string Right) Left, ImmutableArray<ServiceDefinition> Right)> provider
                = syntax.Collect().Combine(assemblyName).Combine(assemblyAttributes);

            context.RegisterSourceOutput(
                provider, (ctx, pair) =>
                {
                    ImmutableArray<ServiceDefinition> servicesFromNamespace = pair.Right;
                    ImmutableArray<ServiceDefinition> servicesFromAttributes = pair.Left.Left;
                    var assemblyName = pair.Left.Right;

                    var servicesToRegister = new HashSet<ServiceDefinition>(servicesFromAttributes, new ServiceDefinitionComparer());

                    // Any explicitly registered services will take precedence over namespace-registered services. This will happen if the user has chosen to change the scope of a service that's also registered in a namespace.
                    foreach (ServiceDefinition service in servicesFromNamespace)
                    {
                        servicesToRegister.Add(service);
                    }

                    Generate(ctx, assemblyName, servicesToRegister);
                }
            );
        }

        private void AddSource(IncrementalGeneratorPostInitializationContext ctx)
        {
            var attributeSourceCode = new StringBuilder().Append(AttributeConstants.SaucyIncludeNamespaceAttribute)
                                                         .AppendTwoNewLines()
                                                         .Append(AttributeConstants.SaucyIncludeAttribute)
                                                         .ToString();

            ctx.AddSource("Saucy.Attributes.g.cs", SourceText.From(attributeSourceCode, Encoding.UTF8));
            ctx.AddSource("Saucy.Enums.g.cs", SourceText.From(EnumConstants.ServiceScopeEnum, Encoding.UTF8));
        }

        private static bool NodeIsClassDeclarationWithSaucyAttributes(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return node is ClassDeclarationSyntax cds
                   && cds.AttributeLists.SelectMany(x => x.Attributes).Any(y => y.Name.ToString() == AttributeConstants.SaucyIncludeAttributeName);
        }

        private static ServiceDefinition GetServiceDetails(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var symbol = (context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol) !;

            AttributeData saucyIncludeAttribute = symbol.GetAttributes().First(x => x.AttributeClass?.Name == AttributeConstants.SaucyIncludeAttributeName);
            var serviceScope = (int)saucyIncludeAttribute.ConstructorArguments[0].Value!;

            return new ServiceDefinition(symbol.GetFullyQualifiedName(), serviceScope, symbol.GetContracts());
        }

        public static void Generate(SourceProductionContext context, string assemblyName, HashSet<ServiceDefinition> servicesToRegister)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var writer = new SourceWriter();

            Dictionary<int, string> serviceScopeEnumValues = new()
            {
                { EnumConstants.SingletonValue, "serviceCollection.AddSingleton" },
                { EnumConstants.TransientValue, "serviceCollection.AddTransient" },
                { EnumConstants.ScopedValue, "serviceCollection.AddScoped" },
            };

            writer.AppendLine($"// <auto-generated by Saucy on {DateTime.Now} />")
                  .AppendLine("using Microsoft.Extensions.DependencyInjection;")
                  .AppendLine()
                  .AppendLine("public static class ServiceCollectionExtensions")
                  .AppendLine("{")
                  .Indent()
                  .AppendLine($"public static IServiceCollection Add{assemblyName}Services(this IServiceCollection services)")
                  .AppendLine("{")
                  .Indent();

            var serviceCount = servicesToRegister.Count;

            if (serviceCount == 0)
            {
                writer.AppendLine("return services;");
            }

            foreach (ServiceDefinition serviceDefinition in servicesToRegister)
            {
                // TODO support keyed services?
                var serviceScope = (int)serviceDefinition.ServiceScope;
                if (serviceDefinition.HasContracts)
                {
                    // Register each contract for the service to support multiple interfaces.
                    foreach (var contractName in serviceDefinition.ContractNames)
                    {
                        writer.AppendLine($"{serviceScopeEnumValues[serviceScope]}<{contractName}, {serviceDefinition.FullyQualifiedClassName}>();");
                    }
                }
                else
                {
                    writer.AppendLine($"{serviceScopeEnumValues[serviceScope]}<{serviceDefinition.FullyQualifiedClassName}>();");
                }
            }

            writer.AppendLine("return services;");

            writer.UnIndent().AppendLine('}').UnIndent().AppendLine('}');

            context.AddSource("SaucyRegistrations.g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
        }
    }
}