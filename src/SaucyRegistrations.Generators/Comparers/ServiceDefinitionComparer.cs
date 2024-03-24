using System.Collections.Generic;

using SaucyRegistrations.Generators.Models;

// ReSharper disable once IdentifierTypo
namespace SaucyRegistrations.Generators.Comparers;

/// <summary>
/// Comparer for <see cref="ServiceDefinition" /> instances.
/// </summary>
internal class ServiceDefinitionComparer : IEqualityComparer<ServiceDefinition>
{
    /// <inheritdoc />
    public bool Equals(ServiceDefinition x, ServiceDefinition y)
    {
        return x.FullyQualifiedClassName == y.FullyQualifiedClassName;
    }

    /// <inheritdoc />
    public int GetHashCode(ServiceDefinition obj)
    {
        return obj.FullyQualifiedClassName.GetHashCode();
    }
}