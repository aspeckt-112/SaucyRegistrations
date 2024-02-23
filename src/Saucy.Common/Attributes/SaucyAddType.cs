using System;

using Saucy.Common.Enums;

namespace Saucy.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SaucyAddType : Attribute
    {
        public ServiceScope ServiceScope { get; }

        public SaucyAddType(ServiceScope serviceScope)
        {
            ServiceScope = serviceScope;
        }
    }
}