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

        Compilation compilation = context.Compilation;

        var entryPoint = compilation.GetEntryPoint(cancellationToken);

        if (entryPoint is null)
        {
            return;
        }

        (string Namespace, string Class, string Method)? generationConfiguration = null;

        var entryPointMethods = entryPoint.ReceiverType.GetMembers().OfType<IMethodSymbol>();

        foreach (var method in entryPointMethods)
        {
            var registrationTargetAttribute = method.FindAttributeOfType<SaucyRegistrationTarget>();

            if (registrationTargetAttribute is not null)
            {
                generationConfiguration = new (entryPoint.ReceiverType.ContainingNamespace.ToDisplayString(), entryPoint.ReceiverType.Name, method.Name);
                break;
            }
        }

        if (generationConfiguration is null)
        {
            return;
        }

        IList<IAssemblySymbol> assembliesToScan = BuildListOfAssembliesToScan(compilation);

        if (!assembliesToScan.Any()
            || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var allNamespacesInAllAssemblies = assembliesToScan.SelectMany(x => x.GlobalNamespace.GetAllNestedNamespaces()).ToList();

        List<IGrouping<ISymbol, INamespaceSymbol>> allNamespacesInAllAssembliesGrouped
            = allNamespacesInAllAssemblies.GroupBy(x => x.ContainingAssembly, SymbolEqualityComparer.Default).ToList();

        Dictionary<ITypeSymbol, ServiceScope> typeSymbols = BuildTypeSymbols(allNamespacesInAllAssembliesGrouped);

        context.AddSource($"{generationConfiguration.Value.Class}_Generated.cs", GenerateRegistrationCode(generationConfiguration!.Value, typeSymbols));
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

    private Dictionary<ITypeSymbol, ServiceScope> BuildTypeSymbols(List<IGrouping<ISymbol, INamespaceSymbol>> assemblyNamespaceGroupings)
    {
        var result = new Dictionary<ITypeSymbol, ServiceScope>(SymbolEqualityComparer.Default);

        foreach (IGrouping<ISymbol, INamespaceSymbol>? grouping in assemblyNamespaceGroupings)
        {
            ISymbol assembly = grouping.Key;
            var namespaces = grouping.ToList();

            var flatListOfTypes
                = (
                    from @namespace in namespaces
                    from type in @namespace.GetInstantiableTypesInNamespace()
                    let excludeTypeAttribute = type.FindAttributeOfType<SaucyExclude>()
                    where excludeTypeAttribute is null
                    select type).ToList();

            // First, get all types that have been explicitly registered.
            var explicitlyRegisteredTypes =
                from type in flatListOfTypes
                let addTypeAttribute = type.FindAttributeOfType<SaucyAddType>()
                where addTypeAttribute is not null
                let scopeAttribute = type.FindAttributeOfType<SaucyScope>()
                where scopeAttribute is not null
                select new { Type = type, ServiceScope = scopeAttribute.GetPropertyValueAsType<ServiceScope>(nameof(SaucyScope.ServiceScope)) };

            foreach (var registeredType in explicitlyRegisteredTypes)
            {
                result.Add(registeredType.Type, registeredType.ServiceScope);
            }

            // Next, get the namespaces that the user has specified to include all types within.
            List<(string Namespace, ServiceScope DefaultServiceScope)> namespacesToIncludeAllTypesWithin = new();

            List<AttributeData> addNamespaceAttributes = assembly.FilterAttributesOfType<SaucyAddNamespace>();

            foreach (AttributeData attribute in addNamespaceAttributes)
            {
                var namespaceToAdd = attribute.GetPropertyValueAsType<string>(nameof(SaucyAddNamespace.Namespace));
                ServiceScope scope = attribute.GetPropertyValueAsType<ServiceScope>(nameof(SaucyAddNamespace.Scope));
                namespacesToIncludeAllTypesWithin.Add((namespaceToAdd, scope));
            }

            foreach ((var namespaceToAdd, ServiceScope scope) in namespacesToIncludeAllTypesWithin)
            {
                var includedNamespaces = namespaces.Where(x => x.ToDisplayString().EndsWith(namespaceToAdd)).ToList();

                foreach (INamespaceSymbol? includedNamespace in includedNamespaces)
                {
                    List<INamedTypeSymbol> typesInNamespace
                        = flatListOfTypes.Where(x => x.ContainingNamespace.ToDisplayString().EndsWith(includedNamespace.ToDisplayString())).ToList();

                    foreach (INamedTypeSymbol? type in typesInNamespace)
                    {
                        // Check to see if the type has a custom scope.
                        AttributeData? scopeAttribute = type.FindAttributeOfType<SaucyScope>();

                        if (scopeAttribute is not null)
                        {
                            ServiceScope customScope = scopeAttribute.GetPropertyValueAsType<ServiceScope>(nameof(SaucyScope.ServiceScope));
                            result.Add(type, customScope);

                            continue;
                        }

                        result.Add(type, scope);
                    }
                }
            }
        }

        return result;
    }

    private string GenerateRegistrationCode((string Namespace, string Class, string Method) generationConfiguration, Dictionary<ITypeSymbol, ServiceScope> typeSymbols)
    {
        StringBuilder sourceBuilder = new();

        sourceBuilder.Append(
            $@"//<auto-generated by Saucy on {DateTime.Now} />
using Microsoft.Extensions.DependencyInjection;

namespace {generationConfiguration.Namespace}
{{
	static partial class {generationConfiguration.Class}
	{{
		static partial void {generationConfiguration.Method}(IServiceCollection serviceCollection)
		{{"
        );

        sourceBuilder.AppendLine();

        Dictionary<ServiceScope, string> serviceScopeToMethodNameMap = new()
        {
            { ServiceScope.Singleton, "serviceCollection.AddSingleton" },
            { ServiceScope.Scoped, "serviceCollection.AddScoped" },
            { ServiceScope.Transient, "serviceCollection.AddTransient" },
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