using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Saucy.Common.Attributes;
using Saucy.Common.Enums;
using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Parameters;

namespace SaucyRegistrations.Generators;

[Generator]
public class SaucyGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		// No initialization required
	}

	public void Execute(GeneratorExecutionContext context)
	{
		RunConfiguration? runParameters = BuildRunConfiguration(context);
		
		if (runParameters is null)
		{
			// TODO Should I do something here?
			return;
		}

		string source = GenerateRegistrationCode(runParameters, context.Compilation.ObjectType);
		
		context.AddSource($"{runParameters.GenerationConfiguration.Class}.Generated.cs", source);
	}
	
	private RunConfiguration? BuildRunConfiguration(
		GeneratorExecutionContext context
	)
	{
		IAssemblySymbol compilationAssembly = context.Compilation.Assembly;

		List<INamespaceSymbol> namespacesInCompilationAssembly
			= compilationAssembly.GlobalNamespace.GetListOfNamespaces().ToList();
		
		if (namespacesInCompilationAssembly.Count == 0)
		{
			throw new InvalidOperationException("No namespaces found in the compilation assembly.");
		}

		GenerationConfiguration? generationConfiguration = BuildGenerationConfiguration(namespacesInCompilationAssembly);

		if (generationConfiguration is null)
		{
			throw new InvalidOperationException(
				"No generation configuration found. Have you applied the [GenerateServiceCollectionMethod] attribute to a class?"
			);
		}
		
		var assemblyScanConfigurations = new
			Dictionary<IAssemblySymbol, AssemblyScanConfiguration>();

		if (compilationAssembly.ShouldBeIncludedInSourceGeneration())
		{
			AddAssemblyScanConfiguration(compilationAssembly, assemblyScanConfigurations);
		}

		List<IAssemblySymbol> referencedAssemblies = context.Compilation
		                                                    .SourceModule
		                                                    .ReferencedAssemblySymbols
		                                                    .Where(x => x.ShouldBeIncludedInSourceGeneration())
		                                                    .ToList();

		foreach (IAssemblySymbol assembly in referencedAssemblies)
		{
			AddAssemblyScanConfiguration(assembly, assemblyScanConfigurations);
		}
		
		// At this point, if there's nothing in the map then there's nothing to generate. So bail out.
		if (assemblyScanConfigurations.Count == 0)
		{
			return null;
		}
		
		Dictionary<ITypeSymbol, ServiceScope> allTypeSymbolsInAllAssemblies
			= GetAllTypeSymbolsInAllAssemblies(assemblyScanConfigurations)

		return new RunConfiguration(generationConfiguration);
		
		
		// IEnumerable<(ITypeSymbol, ServiceScope)> allTypeSymbolsInAllAssemblies
		// 	= GetAllTypeSymbolsInAllAssemblies(assemblyScanConfigurations);
		//
		// runParameter.Types.AddRange(allTypeSymbolsInAllAssemblies);
		//
		// return (diagnostics, runParameter);

	}

	private void AddAssemblyScanConfiguration(IAssemblySymbol compilationAssembly, Dictionary<IAssemblySymbol, AssemblyScanConfiguration> assemblyScanConfigurations)
	{
		AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyScanConfiguration(compilationAssembly);
		assemblyScanConfigurations.Add(compilationAssembly, assemblyScanConfiguration);
	}

	private GenerationConfiguration? BuildGenerationConfiguration(IEnumerable<INamespaceSymbol> namespaceSymbols)
	{
		// Within each namespace, look for the first class with the GenerateServiceCollectionMethodAttribute attribute.
		// If it's found, then use the namespace and class name to build the generation configuration.
		// If it's not found, then return null.
		foreach (INamespaceSymbol namespaceSymbol in namespaceSymbols)
		{
			foreach (INamedTypeSymbol namedTypeSymbol in namespaceSymbol.GetTypeMembers())
			{
				foreach (AttributeData? attribute in namedTypeSymbol.GetAttributes())
				{
					if (attribute.Is<GenerateServiceCollectionMethodAttribute>())
					{
						var methodName = attribute.GetValueOfPropertyWithName<string>(nameof(GenerateServiceCollectionMethodAttribute.MethodName));

						return new GenerationConfiguration(
							namespaceSymbol.ToDisplayString(), namedTypeSymbol.Name, $"{methodName}_Generated"
						);
					}
				}
			}
		}
		
		return null;
	}
	
	private AssemblyScanConfiguration BuildAssemblyScanConfiguration(IAssemblySymbol assemblySymbol)
	{
		AssemblyScanConfiguration result = new();

		List<string> excludedNamespaces = assemblySymbol.GetExcludedNamespaces();

		if (excludedNamespaces.Count > 0)
		{
			result.ExcludedNamespaces.AddRange(excludedNamespaces);
		}

		result.IncludeMicrosoftNamespaces = assemblySymbol.ShouldIncludeMicrosoftNamespaces();
		result.IncludeSystemNamespaces = assemblySymbol.ShouldIncludeSystemNamespaces();
		result.DefaultServiceScope = assemblySymbol.GetDefaultServiceScope();

		return result;
	}
	
	private Dictionary<ITypeSymbol, ServiceScope> GetAllTypeSymbolsInAllAssemblies(
		Dictionary<IAssemblySymbol, AssemblyScanConfiguration> assemblySymbolToAssemblyDetailMap
	)
	{
		Dictionary<ITypeSymbol, ServiceScope> result = new();



		return result;
	}
	

	// private IEnumerable<(ITypeSymbol, ServiceScope)> GetAllTypeSymbolsInAllAssemblies(
	// 	Dictionary<IAssemblySymbol, AssemblyScanConfiguration> assemblySymbolToAssemblyDetailMap
	// )
	// {
	// 	List<(ITypeSymbol, ServiceScope)> result = new();
	//
	// 	foreach (KeyValuePair<IAssemblySymbol, AssemblyScanConfiguration> entry in assemblySymbolToAssemblyDetailMap)
	// 	{
	// 		(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyDetails, List<INamespaceSymbol> namespaceSymbols)
	// 			= GetNamespaceSymbolsFromAssembly(entry.Key, entry.Value);
	//
	// 		foreach (INamespaceSymbol namespaceSymbol in namespaceSymbols)
	// 		{
	// 			IEnumerable<(ITypeSymbol, ServiceScope)> typeSymbolsFromNamespace
	// 				= GetTypeSymbolsFromNamespace(namespaceSymbol, assemblySymbol, assemblyDetails);
	//
	// 			result.AddRange(typeSymbolsFromNamespace);
	// 		}
	// 	}
	//
	// 	return result;
	// }

	private (IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyDetail, List<INamespaceSymbol> namespaceSymbols )
		GetNamespaceSymbolsFromAssembly(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyScanConfiguration)
	{
		throw new NotImplementedException();
		// List<INamespaceSymbol> namespaceSymbols = [];
		//
		// INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
		// List<string> excludedNamespaces = assemblyScanConfiguration.ExcludedNamespaces;
		// bool includeMicrosoftNamespaces = assemblyScanConfiguration.IncludeMicrosoftNamespaces;
		// bool includeSystemNamespaces = assemblyScanConfiguration.IncludeSystemNamespaces;
		//
		// IEnumerable<INamespaceSymbol> namespacesInAssembly = ResolveChildNamespacesRecursively(
		// 	globalNamespace, excludedNamespaces, includeMicrosoftNamespaces, includeSystemNamespaces
		// );
		//
		// namespaceSymbols.AddRange(namespacesInAssembly);
		//
		// return (assemblySymbol, assemblyScanConfiguration, namespaceSymbols);
	}

	private IEnumerable<INamespaceSymbol> ResolveChildNamespacesRecursively(
		INamespaceSymbol namespaceSymbol,
		HashSet<string> excludedNamespaces,
		bool includeMicrosoftNamespaces,
		bool includeSystemNamespaces
	)
	{
		foreach (INamespaceSymbol? symbol in namespaceSymbol.GetNamespaceMembers())
		{
			string symbolDisplayString = symbol.ToDisplayString();

			if (excludedNamespaces.Contains(symbolDisplayString))
			{
				continue;
			}

			if (symbolDisplayString.StartsWith("Microsoft.")
			    && !includeMicrosoftNamespaces)
			{
				continue;
			}

			if (symbolDisplayString.StartsWith("System.")
			    && !includeSystemNamespaces)
			{
				continue;
			}

			yield return symbol;

			foreach (INamespaceSymbol? childNamespace in ResolveChildNamespacesRecursively(
				         symbol, excludedNamespaces, includeMicrosoftNamespaces, includeSystemNamespaces
			         ))
			{
				yield return childNamespace;
			}
		}
	}

	private IEnumerable<(ITypeSymbol, ServiceScope)> GetTypeSymbolsFromNamespace(
		INamespaceSymbol namespaceSymbol,
		IAssemblySymbol assemblySymbol,
		AssemblyScanConfiguration assemblyScanConfiguration
	)
	{
		List<(ITypeSymbol, ServiceScope)> result = [];

		List<INamedTypeSymbol> concreteNamedTypeSymbols = namespaceSymbol.GetTypeMembers()
		                                                                 .Where(x => !x.IsAbstract)
		                                                                 .Where(x => !x.IsStatic)
		                                                                 .ToList();

		if (concreteNamedTypeSymbols.Count == 0)
		{
			return result;
		}

		// Check to see if the assembly has a GenerateRegistrationForClassesWithSuffixAttribute attribute. If it does, then only include types that end with the suffix.
		// If it doesn't, then include all types.
		AttributeData? saucyClassSuffixAttribute
			= assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(GenerateRegistrationForClassesWithSuffixAttribute));

		string? suffix = null;

		if (saucyClassSuffixAttribute is not null)
		{
			suffix = saucyClassSuffixAttribute.GetParameter<string>(nameof(GenerateRegistrationForClassesWithSuffixAttribute.Suffix));
		}

		bool assemblyHasClassSuffix = !string.IsNullOrWhiteSpace(suffix);

		foreach (INamedTypeSymbol namedTypeSymbol in concreteNamedTypeSymbols)
		{
			// First, check to see if the type is decorated with the SaucyExcludeClass attribute.
			// If it is, then just continue to the next type because there's nothing else to do.
			if (namedTypeSymbol.GetFirstAttributeWithNameOrNull(nameof(SaucyExcludeClass)) is not null)
			{
				continue;
			}

			ServiceScope? namedTypeServiceScope = null;

			// Next, check to see if the type is decorated with the SaucyServiceScope attribute.
			// If it is, then use the value from the attribute. If it's not, then use the default value from the assembly.
			AttributeData? saucyServiceScopeAttribute
				= namedTypeSymbol.GetFirstAttributeWithNameOrNull(nameof(SaucyClassScope));

			if (saucyServiceScopeAttribute is not null)
			{
				namedTypeServiceScope = saucyServiceScopeAttribute.GetParameter<ServiceScope>(
					nameof(SaucyClassScope.ServiceScope)
				);
			}

			ServiceScope serviceScope = namedTypeServiceScope ?? assemblyScanConfiguration.DefaultServiceScope;

			if (assemblyHasClassSuffix)
			{
				if (namedTypeSymbol.Name.EndsWith(suffix!))
				{
					result.Add((namedTypeSymbol, serviceScope));
				}

				continue;
			}

			result.Add((namedTypeSymbol, serviceScope));
		}

		return result;
	}

	private string GenerateRegistrationCode(RunConfiguration runConfiguration, INamedTypeSymbol objectSymbol)
	{
		var sourceBuilder = new StringBuilder();
		
		GenerationConfiguration generationConfiguration = runConfiguration.GenerationConfiguration;

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

		// Dictionary<ServiceScope, string> serviceScopeToMethodNameMap = new()
		// {
		// 	{ ServiceScope.Singleton, "serviceCollection.AddSingleton" },
		// 	{ ServiceScope.Scoped, "serviceCollection.AddScoped" },
		// 	{ ServiceScope.Transient, "serviceCollection.AddTransient" }
		// };
		//
		// foreach ((ITypeSymbol typeSymbol, ServiceScope typeScope) in runParameter.Types)
		// {
		// 	string fullyQualifiedTypeName = typeSymbol.ToDisplayString();
		//
		// 	// Check to see if the class has a base type that's not "object". If it does, then also check to see if the base type is abstract.
		// 	// If it is, also register the concrete type against the base type to support resolving by the base type.
		// 	INamedTypeSymbol? classBaseType = typeSymbol.BaseType;
		//
		// 	bool classHasBaseType = classBaseType is not null && !ReferenceEquals(classBaseType, objectSymbol);
		//
		// 	if (classHasBaseType && typeSymbol.BaseType!.IsAbstract)
		// 	{
		// 		string fullyQualifiedBaseTypeName = typeSymbol.BaseType.ToDisplayString();
		//
		// 		sourceBuilder.AppendLine(
		// 			$@"			{serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedBaseTypeName}, {fullyQualifiedTypeName}>();"
		// 		);
		// 	}
		//
		// 	bool classHasInterfaces = typeSymbol.Interfaces.Length > 0;
		//
		// 	switch (classHasInterfaces)
		// 	{
		// 		// If the class has interfaces, then register the concrete type against each interface to support resolving by
		// 		// the interface type.
		// 		case true:
		// 		{
		// 			foreach (INamedTypeSymbol @interface in typeSymbol.Interfaces)
		// 			{
		// 				string fullyQualifiedInterfaceName = @interface.ToDisplayString();
		//
		// 				sourceBuilder.AppendLine(
		// 					$@"			{serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedInterfaceName}, {fullyQualifiedTypeName}>();"
		// 				);
		// 			}
		//
		// 			break;
		// 		}
		// 		// If the class has no interfaces, and no base type, then just register the concrete type against itself.
		// 		case false 
		// 	    when !classHasBaseType:
		// 			sourceBuilder.AppendLine($@"			{serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedTypeName}>();");
		// 			break;
		// 	}
		// }

		sourceBuilder.AppendLine(@"		}");
		sourceBuilder.AppendLine(@"	}");
		sourceBuilder.AppendLine(@"}");

		return sourceBuilder.ToString();
	}
}
