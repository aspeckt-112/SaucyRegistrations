using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: IncludeInSourceGenerationRegistrationWithDefaultServiceScope(ServiceScope.Transient)]
[assembly: GenerateRegistrationForClassesWithSuffix("Repository")]
[assembly: GenerateRegistrationForClassesWithSuffix("Helper")]
