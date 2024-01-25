using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: SourceGenerationTarget("Saucy.Console", "Program", "AddRegistrationsInternal")]
[assembly: IncludeInSourceGeneration(ServiceScope.Transient)]
[assembly: SaucyClassSuffix("Service")]
