using System;
using System.Text;

namespace SaucyRegistrations.Generators.Infrastructure;

/// <summary>
/// A class that writes source code.
/// </summary>
internal sealed class SourceWriter
{
    private readonly StringBuilder _sb = new();

    private int _indentation;

    /// <inheritdoc />
    public override string ToString() => _sb.ToString();

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter Indent()
    {
        _indentation++;

        return this;
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter UnIndent()
    {
        _indentation = Math.Max(0, _indentation - 1);

        return this;
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter AppendLine()
    {
        _sb.AppendLine();

        return this;
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter AppendLine(char value)
    {
        _sb.Append(new string(' ', _indentation * 4)).Append(value).AppendLine();

        return this;
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter AppendLine(string value)
    {
        _sb.Append(new string(' ', _indentation * 4)).AppendLine(value);

        return this;
    }

    /// <summary>
    /// Appends a string (without a new line) to the source code.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter Append(char value)
    {
        _sb.Append(value);

        return this;
    }
}