using System.Collections.Generic;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Configurations;

public class AssemblyScanConfiguration
{
	public HashSet<string> ExcludedNamespaces { get; private set; } = new();

	public bool IncludeMicrosoftNamespaces { get; set; }

	public bool IncludeSystemNamespaces { get; set; }
	
	public ServiceScope DefaultServiceScope { get; set; }
}
