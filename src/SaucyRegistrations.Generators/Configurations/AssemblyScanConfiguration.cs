using System.Collections.Generic;

using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Configurations;

/// <summary>
/// The configuration for the assembly scanning.
/// </summary>
public class AssemblyScanConfiguration
{
    /// <summary>
    /// Gets the included namespaces.
    /// </summary>
    /// <remarks>Always a new list by default. You can assume this is never null.</remarks>
    public List<string> ExcludedNamespaces { get; private set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to include Microsoft namespaces.
    /// </summary>
    public bool IncludeMicrosoftNamespaces { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include system namespaces.
    /// </summary>
    public bool IncludeSystemNamespaces { get; set; }

    /// <summary>
    /// Gets or sets the default service scope.
    /// </summary>
    public ServiceScope DefaultServiceScope { get; set; }
}