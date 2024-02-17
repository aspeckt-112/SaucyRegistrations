using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: DefaultScopeRegistration(ServiceScope.Transient)]
[assembly: SuffixRegistration("Service")]
[assembly: SuffixRegistration("Builder")]
[assembly: ExcludeNamespace("Saucy.Console.ExcludedNamespace")]