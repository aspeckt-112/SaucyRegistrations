using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: IncludeInSourceGeneration(ServiceScope.Transient)]
[assembly: SaucyExcludeNamespace("ExampleAssemblyOfReusableComponents.EverythingInThisNamespaceWillBeExcluded")]
[assembly: SaucyClassSuffix("ThisSuffix")]