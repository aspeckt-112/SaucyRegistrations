using System;
using System.Diagnostics;

namespace SaucyRegistrations.Generators.Logging;

/// <summary>
/// The <see cref="Logger"/> for the Saucy source generator.
/// </summary>
internal class Logger
{
    /// <summary>
    /// Writes an information message to the logger.
    /// </summary>
    /// <param name="message">The message to write to the logger.</param>
    /// <remarks>Information messages are written in green.</remarks>
    /// <remarks>Information messages are written to the debug output.</remarks>
    /// <remarks>Information messages are written to the console in debug mode.</remarks>
    internal void WriteInformation(string message)
    {
        var formattedMessage = $"[Saucy].[INFO] {message}";

        #if DEBUG
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(formattedMessage);
        Console.ResetColor();

        #endif

        Debug.WriteLine(formattedMessage);
    }

    /// <summary>
    /// Writes a warning message to the logger.
    /// </summary>
    /// <param name="message">The message to write to the logger.</param>
    /// <remarks>Warnings are written in yellow.</remarks>
    /// <remarks>Warnings are written to the debug output.</remarks>
    /// <remarks>Warnings are written to the console in debug mode.</remarks>
    internal void WriteWarning(string message)
    {
        var formattedMessage = $"[Saucy].[WARN] {message}";

        #if DEBUG
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(formattedMessage);
        Console.ResetColor();

        #endif

        Debug.WriteLine(formattedMessage);
    }
}