using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Saucy.Common.Attributes;
using Saucy.Common.Enums;
using SaucyRegistrations.Generators.Extensions;

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
		context.ReportDiagnostic(Diagnostics.StartingSaucySourceGeneration);

		(List<Diagnostic> diagnostics, RunParameters? runParameters) = BuildRunParameters(context);

		// Some diagnostics might have been added during the generation process.
		// They're not necessarily errors, so just write them out.
		// The nullability of runParameters indicates whether or not the generation process was successful.
		if (diagnostics.Count > 0)
		{
			foreach (Diagnostic diagnostic in diagnostics)
			{
				context.ReportDiagnostic(diagnostic);
			}

			return;
		}

		if (runParameters is null)
		{
			// Just return, there's nothing to do and all the diagnostics have already been added.
			return;
		}

		string source = GenerateRegistrationCode(runParameters, context.Compilation.ObjectType);

		context.AddSource($"{runParameters.PartialClass}.Generated.cs", source);
	}

	private (List<Diagnostic> diagnostics, RunParameters? runParameters) BuildRunParameters(
		GeneratorExecutionContext context
	)
	{
		List<Diagnostic> diagnostics = [];

		IAssemblySymbol compilationAssembly = context.Compilation.Assembly;

		AttributeData? saucyCompilationTargetAttribute
			= compilationAssembly.GetFirstAttributeWithNameOrNull(nameof(SourceGenerationTarget));

		// SourceGenerationTarget is mandatory. If it's not found, then there's no point in continuing.
		if (saucyCompilationTargetAttribute is null)
		{
			diagnostics.Add(Diagnostics.SaucyTargetAttributeNotFound);
			return (diagnostics, null);
		}

		(string? @namespace, string? partialClass, string? generationMethod)
			= GetDetailsFromSaucyCompilationTarget(saucyCompilationTargetAttribute);

		// It's mandatory to have a namespace, partial class name and generation method name. If any is missing, then there's no point in continuing.
		if (AllStringsOrNullOrEmpty(@namespace, partialClass, generationMethod))
		{
			diagnostics.Add(Diagnostics.SaucyTargetAttributeMissingProperties);
			return (diagnostics, null);
		}

		RunParameters runParameters = new(@namespace, partialClass, generationMethod);

		Dictionary<IAssemblySymbol, AssemblyScanConfiguration> assemblyScanConfigurations = new();

		if (compilationAssembly.HasSaucyIncludeAttribute())
		{
			AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyDetail(compilationAssembly);
			assemblyScanConfigurations.Add(compilationAssembly, assemblyScanConfiguration);
		}

		ImmutableArray<IAssemblySymbol> referencedAssemblySymbols = context.Compilation
		                                                                   .SourceModule.ReferencedAssemblySymbols;

		List<IAssemblySymbol> assembliesToScan
			= referencedAssemblySymbols.Where(x => x.HasSaucyIncludeAttribute()).ToList();

		foreach (IAssemblySymbol assemblySymbol in assembliesToScan)
		{
			AssemblyScanConfiguration assemblyScanConfiguration = BuildAssemblyDetail(assemblySymbol);
			assemblyScanConfigurations.Add(assemblySymbol, assemblyScanConfiguration);
		}

		// At this point, if there's nothing in the map then there's nothing to generate. So bail out.
		if (assemblyScanConfigurations.Count == 0)
		{
			diagnostics.Add(Diagnostics.NoAssembliesToScan);
			return (diagnostics, null);
		}

		IEnumerable<(ITypeSymbol, ServiceScope)> allTypeSymbolsInAllAssemblies
			= GetAllTypeSymbolsInAllAssemblies(assemblyScanConfigurations);

		runParameters.Types.AddRange(allTypeSymbolsInAllAssemblies);

		return (diagnostics, runParameters);
	}

	private (string @namespace, string partialClass, string generationMethod) GetDetailsFromSaucyCompilationTarget(
		AttributeData saucyCompilationTargetAttribute
	)
	{
		var @namespace = saucyCompilationTargetAttribute.GetParameter<string>(nameof(SourceGenerationTarget.Namespace));

		var partialClass
			= saucyCompilationTargetAttribute.GetParameter<string>(nameof(SourceGenerationTarget.PartialClass));

		var generationMethod
			= saucyCompilationTargetAttribute.GetParameter<string>(nameof(SourceGenerationTarget.GenerationMethod));

		return (@namespace, partialClass, generationMethod);
	}

	private bool AllStringsOrNullOrEmpty(params string[] strings) => strings.All(string.IsNullOrWhiteSpace);

	private AssemblyScanConfiguration BuildAssemblyDetail(IAssemblySymbol assemblySymbol)
	{
		AssemblyScanConfiguration result = new();

		List<string> excludedNamespaces = assemblySymbol.GetExcludedNamespaces();

		if (excludedNamespaces.Count > 0)
		{
			result.ExcludedNamespaces.UnionWith(excludedNamespaces);
		}

		result.IncludeMicrosoftNamespaces = assemblySymbol.ShouldIncludeMicrosoftNamespaces();
		result.IncludeSystemNamespaces = assemblySymbol.ShouldIncludeSystemNamespaces();
		result.DefaultServiceScope = assemblySymbol.GetDefaultServiceScope();

		return result;
	}

	private IEnumerable<(ITypeSymbol, ServiceScope)> GetAllTypeSymbolsInAllAssemblies(
		Dictionary<IAssemblySymbol, AssemblyScanConfiguration> assemblySymbolToAssemblyDetailMap
	)
	{
		List<(ITypeSymbol, ServiceScope)> result = new();

		foreach (KeyValuePair<IAssemblySymbol, AssemblyScanConfiguration> entry in assemblySymbolToAssemblyDetailMap)
		{
			(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyDetails, List<INamespaceSymbol> namespaceSymbols)
				= GetNamespaceSymbolsFromAssembly(entry.Key, entry.Value);

			foreach (INamespaceSymbol namespaceSymbol in namespaceSymbols)
			{
				IEnumerable<(ITypeSymbol, ServiceScope)> typeSymbolsFromNamespace
					= GetTypeSymbolsFromNamespace(namespaceSymbol, assemblySymbol, assemblyDetails);

				result.AddRange(typeSymbolsFromNamespace);
			}
		}

		return result;
	}

	private (IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyDetail, List<INamespaceSymbol> namespaceSymbols )
		GetNamespaceSymbolsFromAssembly(IAssemblySymbol assemblySymbol, AssemblyScanConfiguration assemblyScanConfiguration)
	{
		List<INamespaceSymbol> namespaceSymbols = [];

		INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
		HashSet<string> excludedNamespaces = assemblyScanConfiguration.ExcludedNamespaces;
		bool includeMicrosoftNamespaces = assemblyScanConfiguration.IncludeMicrosoftNamespaces;
		bool includeSystemNamespaces = assemblyScanConfiguration.IncludeSystemNamespaces;

		IEnumerable<INamespaceSymbol> namespacesInAssembly = ResolveChildNamespacesRecursively(
			globalNamespace, excludedNamespaces, includeMicrosoftNamespaces, includeSystemNamespaces
		);

		namespaceSymbols.AddRange(namespacesInAssembly);

		return (assemblySymbol, assemblyScanConfiguration, namespaceSymbols);
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

		// Check to see if the assembly has a SaucyClassSuffix attribute. If it does, then only include types that end with the suffix.
		// If it doesn't, then include all types.
		AttributeData? saucyClassSuffixAttribute
			= assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(SaucyClassSuffix));

		string? suffix = null;

		if (saucyClassSuffixAttribute is not null)
		{
			suffix = saucyClassSuffixAttribute.GetParameter<string>(nameof(SaucyClassSuffix.Suffix));
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

	private string GenerateRegistrationCode(RunParameters runParameters, INamedTypeSymbol objectSymbol)
	{
		var sourceBuilder = new StringBuilder();

		sourceBuilder.Append(
			$@"//<auto-generated by Saucy on {DateTime.Now} />
using Microsoft.Extensions.DependencyInjection;

namespace {runParameters.Namespace}
{{
 public static partial class {runParameters.PartialClass}
 {{
  public static void {runParameters.GenerationMethod}(IServiceCollection serviceCollection)
  {{"
		);

		sourceBuilder.AppendLine();

		Dictionary<ServiceScope, string> serviceScopeToMethodNameMap = new()
		{
			{ ServiceScope.Singleton, "serviceCollection.AddSingleton" },
			{ ServiceScope.Scoped, "serviceCollection.AddScoped" },
			{ ServiceScope.Transient, "serviceCollection.AddTransient" }
		};

		foreach ((ITypeSymbol typeSymbol, ServiceScope typeScope) in runParameters.Types)
		{
			string fullyQualifiedTypeName = typeSymbol.ToDisplayString();

			// Check to see if the class has a base type that's not "object". If it does, then also check to see if the base type is abstract.
			// If it is, also register the concrete type against the base type to support resolving by the base type.
			INamedTypeSymbol? classBaseType = typeSymbol.BaseType;

			bool classHasBaseType = classBaseType is not null && !ReferenceEquals(classBaseType, objectSymbol);

			if (classHasBaseType && typeSymbol.BaseType!.IsAbstract)
			{
				string fullyQualifiedBaseTypeName = typeSymbol.BaseType.ToDisplayString();

				sourceBuilder.AppendLine(
					$@"			{serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedBaseTypeName}, {fullyQualifiedTypeName}>();"
				);
			}

			bool classHasInterfaces = typeSymbol.Interfaces.Length > 0;

			switch (classHasInterfaces)
			{
				// If the class has interfaces, then register the concrete type against each interface to support resolving by
				// the interface type.
				case true:
				{
					foreach (INamedTypeSymbol @interface in typeSymbol.Interfaces)
					{
						string fullyQualifiedInterfaceName = @interface.ToDisplayString();

						sourceBuilder.AppendLine(
							$@"			{serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedInterfaceName}, {fullyQualifiedTypeName}>();"
						);
					}

					break;
				}
				// If the class has no interfaces, and no base type, then just register the concrete type against itself.
				case false 
			    when !classHasBaseType:
					sourceBuilder.AppendLine($@"			{serviceScopeToMethodNameMap[typeScope]}<{fullyQualifiedTypeName}>();");
					break;
			}
		}

		sourceBuilder.AppendLine(@"		}");
		sourceBuilder.AppendLine(@"	}");
		sourceBuilder.AppendLine(@"}");

		return sourceBuilder.ToString();
	}
}
