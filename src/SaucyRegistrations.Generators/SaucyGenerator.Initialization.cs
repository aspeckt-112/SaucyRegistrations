using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using SaucyRegistrations.Generators.Builders;
using SaucyRegistrations.Generators.SourceConstants.Attributes;
using SaucyRegistrations.Generators.SourceConstants.Enums;

namespace SaucyRegistrations.Generators;

/// <summary>
/// The initialization for the Saucy generator.
/// </summary>
public partial class SaucyGenerator
{
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
}