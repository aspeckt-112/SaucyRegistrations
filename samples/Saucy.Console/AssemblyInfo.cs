using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: IncludeInSourceGenerationRegistrationWithDefaultServiceScope(ServiceScope.Transient)]
[assembly: GenerateRegistrationForClassesWithSuffix("Service")]
[assembly: WhenRegisteringExcludeClassesInNamespace("Saucy.Console.ExcludedNamespace")]