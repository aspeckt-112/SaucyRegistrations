using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: DefaultScopeRegistration(ServiceScope.Transient)]
[assembly: SuffixRegistration("Repository")]
[assembly: SuffixRegistration("Helper")]
