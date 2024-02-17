using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Collections;

/// <summary>
/// A collection of <see cref="INamespaceSymbol"/>.
/// </summary>
#pragma warning disable SA1106
public class Namespaces : List<INamespaceSymbol>;
#pragma warning restore SA1106
