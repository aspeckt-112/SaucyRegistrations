using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Extensions;

namespace SaucyRegistrations.Generators;

/// <summary>
/// The source generator for the Saucy library.
/// </summary>
[Generator]
public sealed class SaucyGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;

        IList<IAssemblySymbol> assembliesToScan = BuildListOfAssembliesToScan(context.Compilation);

        if (!assembliesToScan.Any() || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var allNamespacesInAllAssemblies = assembliesToScan.SelectMany(x => x.GlobalNamespace.GetNamespaces()).ToList();

        (string Namespace, string Class, string Method)? generationConfiguration = BuildGenerationConfiguration(allNamespacesInAllAssemblies);

        if (generationConfiguration is null || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        // Create an enumerable grouping of all namespaces in all assemblies, to prevent the need to iterate over the same namespaces multiple times.
        var allNamespacesInAllAssembliesGrouped = allNamespacesInAllAssemblies.GroupBy(x => x.ContainingAssembly, SymbolEqualityComparer.Default).ToList();

        var typeSymbols = new Dictionary<ITypeSymbol, ServiceScope>(SymbolEqualityComparer.Default);

        foreach (IAssemblySymbol assemblySymbol in assembliesToScan)
        {
            var assemblyNamespaces = allNamespacesInAllAssembliesGrouped.First(x => SymbolEqualityComparer.Default.Equals(x.Key, assemblySymbol)).ToList();

            var flatListOfTypes
                = (
                    from @namespace in assemblyNamespaces
                    from type in @namespace.GetConcreteTypes()
                    let excludeTypeAttribute = type.GetFirstAttributeOfType<SaucyExclude>()
                    where excludeTypeAttribute is null
                    select type).ToList();

            // First, get all types that have been explicitly registered.
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

            // Next, get the namespaces that the user has specified to include all types within.
            List<(string, ServiceScope)> namespacesToIncludeAllTypesWithin = new();

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

                foreach (INamespaceSymbol? includedNamespace in includedNamespaces)
                {
                    List<INamedTypeSymbol> typesInNamespace = flatListOfTypes.Where(x => x.ContainingNamespace.ToDisplayString().EndsWith(includedNamespace.ToDisplayString())).ToList();

                    foreach (INamedTypeSymbol? type in typesInNamespace)
                    {
                        // Check to see if the type has a custom scope.
                        AttributeData? scopeAttribute = type.GetFirstAttributeOfType<SaucyScope>();

                        if (scopeAttribute is not null)
                        {
                            ServiceScope customScope = scopeAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(SaucyScope.ServiceScope));
                            typeSymbols.Add(type, customScope);

                            continue;
                        }

                        typeSymbols.Add(type, scope);
                    }
                }
            }
        }

        var registrationCode = GenerateRegistrationCode(generationConfiguration!.Value, typeSymbols);

        context.AddSource("SaucyRegistrations_Generated.cs", registrationCode);
    }

    private IList<IAssemblySymbol> BuildListOfAssembliesToScan(Compilation compilation)
    {
        var assemblies = new List<IAssemblySymbol>();

        IAssemblySymbol compilationAssembly = compilation.Assembly;

        if (compilationAssembly.ShouldBeIncludedInSourceGeneration())
        {
            assemblies.Add(compilationAssembly);
        }

        var referencedAssemblies = compilation.SourceModule.ReferencedAssemblySymbols.Where(x => x.ShouldBeIncludedInSourceGeneration()).ToList();

        assemblies.AddRange(referencedAssemblies);

        return assemblies;
    }

    private (string Namespace, string Class, string Method)? BuildGenerationConfiguration(IList<INamespaceSymbol> namespaces)
    {
        foreach (INamespaceSymbol? assemblyNamespace in namespaces)
        {
            foreach (INamedTypeSymbol? namedTypeSymbol in assemblyNamespace.GetTypeMembers())
            {
                foreach (AttributeData? attribute in namedTypeSymbol.GetAttributes())
                {
                    if (attribute.Is<ServiceCollectionMethod>())
                    {
                        var methodName = attribute.GetValueForPropertyOfType<string>(nameof(ServiceCollectionMethod.MethodName));

                        return new ValueTuple<string, string, string>(assemblyNamespace.ToDisplayString(), namedTypeSymbol.Name, methodName);
                    }
                }
            }
        }

        return null;
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
            { ServiceScope.Transient, "serviceCollection.AddTransient" }
        };

        foreach (KeyValuePair<ITypeSymbol, ServiceScope> type in typeSymbols)
        {
            ITypeSymbol typeSymbol = type.Key;
            ServiceScope typeScope = type.Value;

            var fullyQualifiedTypeName = typeSymbol.ToDisplayString();

            INamedTypeSymbol? classBaseType = typeSymbol.BaseType;

            var classHasAbstractBaseType = classBaseType is not null && classBaseType.IsAbstract;

            if (classHasAbstractBaseType)
            {
                var fullyQualifiedBaseTypeName = typeSymbol.BaseType!.ToDisplayString();

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
                case false when !classHasAbstractBaseType:
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