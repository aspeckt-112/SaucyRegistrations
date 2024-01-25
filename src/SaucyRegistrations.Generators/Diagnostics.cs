using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators;

internal static class Diagnostics
{
#region Info

	internal static Diagnostic StartingSaucySourceGeneration
	{
		get => Diagnostic.Create(
			new DiagnosticDescriptor(
				"Saucy1001", "Starting Saucy Source Generation", "Starting Saucy Source Generation",
				"SaucyRegistrations.Generators", DiagnosticSeverity.Info, true
			), Location.None
		);
	}

	internal static Diagnostic NoAssembliesToScan
	{
		get => Diagnostic.Create(
			new DiagnosticDescriptor(
				"Saucy1002", "No Assemblies To Scan", "No Assemblies To Scan", "SaucyRegistrations.Generators",
				DiagnosticSeverity.Info, true
			), Location.None
		);
	}

	internal static Diagnostic NoNamespacesToScan
	{
		get => Diagnostic.Create(
			new DiagnosticDescriptor(
				"Saucy1003", "No Namespaces To Scan", "No Namespaces To Scan", "SaucyRegistrations.Generators",
				DiagnosticSeverity.Info, true
			), Location.None
		);
	}

#endregion

#region Error

	internal static Diagnostic SaucyTargetAttributeNotFound
	{
		get => Diagnostic.Create(
			new DiagnosticDescriptor(
				"Saucy2001", "SaucyTarget Attribute Not Found", "SaucyTarget Attribute Not Found",
				"SaucyRegistrations.Generators", DiagnosticSeverity.Error, true
			), Location.None
		);
	}

	internal static Diagnostic SaucyTargetAttributeMissingProperties
	{
		get => Diagnostic.Create(
			new DiagnosticDescriptor(
				"Saucy2002", "SaucyTarget Attribute Missing Properties", "SaucyTarget Attribute Missing Properties",
				"SaucyRegistrations.Generators", DiagnosticSeverity.Error, true
			), Location.None
		);
	}

#endregion
}
