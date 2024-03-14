using System.Collections.Generic;

using SaucyRegistrations.Generators.Models;

namespace SaucyRegistrations.Generators.Comparers
{
    public class ServiceDefinitionComparer : IEqualityComparer<ServiceDefinition>
    {
        public bool Equals(ServiceDefinition x, ServiceDefinition y)
        {
            return x.FullyQualifiedClassName == y.FullyQualifiedClassName;
        }

        public int GetHashCode(ServiceDefinition obj)
        {
            return obj.FullyQualifiedClassName.GetHashCode();
        }
    }
}