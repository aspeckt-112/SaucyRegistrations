using System.Collections.Generic;
using System.ComponentModel;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Configurations;

public class AssemblyScanConfiguration
{
	public List<string> ExcludedNamespaces { get; private set; } = new();

	public bool IncludeMicrosoftNamespaces { get; set; }

	public bool IncludeSystemNamespaces { get; set; }
	
	public ServiceScope DefaultServiceScope { get; set; }
}
