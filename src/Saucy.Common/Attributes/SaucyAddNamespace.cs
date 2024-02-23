using System;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class SaucyAddNamespace : Attribute
    {
        public SaucyAddNamespace(string @namespace, ServiceScope scope)
        {
            Namespace = @namespace;
            Scope = scope;
        }

        public string Namespace { get; }
        public ServiceScope Scope { get; }
    }
}