using Microsoft.CodeAnalysis;

using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Models;

public class Type
{
    public ITypeSymbol Symbol { get; set; }

    public ServiceScope ServiceScope { get; set; }
}