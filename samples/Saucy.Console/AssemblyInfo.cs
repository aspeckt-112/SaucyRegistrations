﻿[assembly: SaucyIncludeNamespace(nameof(Saucy.Console.Services), ServiceScope.Transient)]
[assembly: SaucyIncludeNamespace(nameof(Saucy.Console.Builders), ServiceScope.Singleton)]
[assembly: SaucyIncludeNamespace(nameof(Saucy.Console.Keyed), ServiceScope.Scoped)]