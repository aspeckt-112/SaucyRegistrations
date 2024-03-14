using System.Collections.Generic;

namespace SaucyRegistrations.Generators.Models
{
    public class ServiceDefinition(string fullQualifiedClassName, int? serviceScope, List<string>? contractNames)
    {
        public string FullyQualifiedClassName { get; } = fullQualifiedClassName;

        public int? ServiceScope { get; } = serviceScope;

        public List<string>? ContractNames { get; } = contractNames;

        public bool HasContracts => ContractNames?.Count > 0;
    }
}