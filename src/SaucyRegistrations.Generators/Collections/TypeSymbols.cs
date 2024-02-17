using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Saucy.Common.Enums;

using SaucyRegistrations.Generators.Models;

namespace SaucyRegistrations.Generators.Collections;

/// <summary>
/// A collection of <see cref="ITypeSymbol" /> and <see cref="ServiceScope" />.
/// </summary>
/// <remarks>Used to store the <see cref="ServiceScope" /> of a <see cref="ITypeSymbol" />.</remarks>
/// <seealso cref="ITypeSymbol" />
/// <seealso cref="ServiceScope" />
public class TypeSymbols : List<Type> { }