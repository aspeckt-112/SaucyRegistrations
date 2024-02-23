// using System.Collections.Generic;
//
// using Microsoft.CodeAnalysis;
//
// using Saucy.Common.Attributes;
// using Saucy.Common.Enums;
//
// using SaucyRegistrations.Generators.Configurations;
// using SaucyRegistrations.Generators.Extensions;
//
// namespace SaucyRegistrations.Generators.Builders;
//
// /// <summary>
// /// A builder for the <see cref="AssemblyScanConfiguration" /> class.
// /// </summary>
// internal static class AssemblyScanConfigurationBuilder
// {
//     /// <summary>
//     /// Builds an <see cref="AssemblyScanConfiguration" /> object from the provided <see cref="IAssemblySymbol" />.
//     /// </summary>
//     /// <param name="assembly">The <see cref="IAssemblySymbol" /> to build the <see cref="AssemblyScanConfiguration" /> from.</param>
//     /// <param name="serviceScope">The <see cref="ServiceScope" /> to use as the default service scope.</param>
//     /// <returns>An <see cref="AssemblyScanConfiguration" /> object.</returns>
//     internal static AssemblyScanConfiguration Build(IAssemblySymbol assembly, ServiceScope? serviceScope)
//     {
//         AssemblyScanConfiguration result = new();
//
//         List<string> excludedNamespaces = assembly.GetIncludedNamespaces();
//
//         if (excludedNamespaces.Count > 0)
//         {
//             result.ExcludedNamespaces.AddRange(excludedNamespaces);
//         }
//
//         result.IncludeMicrosoftNamespaces = assembly.ShouldIncludeMicrosoftNamespaces();
//         result.IncludeSystemNamespaces = assembly.ShouldIncludeSystemNamespaces();
//         result.DefaultServiceScope = (ServiceScope)serviceScope!;
//
//         List<AttributeData> classSuffixAttributes = assembly.GetAttributesOfType<SuffixRegistration>();
//
//         if (classSuffixAttributes.Count > 0)
//         {
//             List<string> classSuffixes = new();
//
//             foreach (AttributeData attribute in classSuffixAttributes)
//             {
//                 var classSuffix = attribute.GetValueForPropertyOfType<string>(nameof(SuffixRegistration.Suffix));
//
//                 if (!string.IsNullOrWhiteSpace(classSuffix))
//                 {
//                     classSuffixes.Add(classSuffix);
//                 }
//             }
//
//             result.ClassSuffixes.AddRange(classSuffixes);
//         }
//
//         return result;
//     }
// }