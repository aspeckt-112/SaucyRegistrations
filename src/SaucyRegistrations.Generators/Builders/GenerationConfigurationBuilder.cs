using System.Linq;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;

using SaucyRegistrations.Generators.Configurations;
using SaucyRegistrations.Generators.Extensions;
using SaucyRegistrations.Generators.Logging;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// The builder for the <see cref="GenerationConfiguration" /> class.
/// </summary>
internal class GenerationConfigurationBuilder
{
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationConfigurationBuilder" /> class.
    /// </summary>
    /// <param name="logger">The <see cref="Logger"/>.</param>
    internal GenerationConfigurationBuilder(Logger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Builds the generation configuration.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <returns>
    /// An instance of the <see cref="GenerationConfiguration" /> class, or null if there's no generation
    /// configuration found.
    /// </returns>
    internal GenerationConfiguration? Build(Compilation compilation)
    {
        IAssemblySymbol compilationAssembly = compilation.Assembly;

        _logger.WriteInformation($"Building generation configuration for assembly: {compilationAssembly.Name}");

        var compilationAssemblyNamespaces = compilationAssembly.GlobalNamespace.GetNamespaces().ToList();

        if (compilationAssemblyNamespaces.Count == 0)
        {
            _logger.WriteInformation("No namespaces found in the compilation assembly.");
            return null;
        }

        GenerationConfiguration? generationConfiguration = null;

        // We need to find the first class with the ServiceCollectionMethod attribute.
        // For example, Program.cs in a console application. App.xaml.cs in a desktop application, etc.
        foreach (INamespaceSymbol? @namespace in compilationAssemblyNamespaces)
        {
            foreach (INamedTypeSymbol? namedTypeSymbol in @namespace.GetTypeMembers())
            {
                foreach (AttributeData? attribute in namedTypeSymbol.GetAttributes())
                {
                    if (attribute.Is<ServiceCollectionMethod>())
                    {
                        _logger.WriteInformation($"Found class with ServiceCollectionMethod attribute: {namedTypeSymbol.Name}");
                        var methodName = attribute.GetValueForPropertyOfType<string>(nameof(ServiceCollectionMethod.MethodName));
                        generationConfiguration = new GenerationConfiguration(@namespace.ToDisplayString(), namedTypeSymbol.Name, methodName);
                    }
                }
            }
        }

        if (generationConfiguration is null)
        {
            _logger.WriteInformation("No class with ServiceCollectionMethod attribute found.");
        }

        return generationConfiguration;
    }
}