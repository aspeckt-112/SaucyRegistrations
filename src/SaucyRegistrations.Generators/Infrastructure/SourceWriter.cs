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
    /// <param name="value">The string to append.</param>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    internal SourceWriter AppendLine(string value)
    {
        _sb.Append(new string(' ', _indentation * 4)).AppendLine(value);

        return this;
    }

    /// <summary>
    /// Appends a header to the source code.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly.</param>
    /// <param name="className">The name of the class.</param>
    internal void AppendHeader(string assemblyName, string className)
    {
        this.AppendLine("// <auto-generated by Saucy. DO NOT CHANGE THIS FILE!!! />")
            .AppendLine("using Microsoft.Extensions.DependencyInjection;")
            .AppendLine()
            .AppendLine($"namespace {assemblyName}.ServiceCollectionExtensions;")
            .AppendLine()
            .AppendLine($"public static class {className}")
            .AppendLine("{")
            .Indent()
            .AppendLine($"public static IServiceCollection Add{assemblyName.Replace(".", string.Empty)}Services(this IServiceCollection services)")
            .AppendLine("{")
            .Indent();
    }

    /// <summary>
    /// Appends a footer to the source code.
    /// </summary>
    internal void AppendFooter()
    {
        this.AppendLine("return services;")
            .UnIndent()
            .AppendLine("}")
            .UnIndent()
            .Append('}');
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    private SourceWriter Indent()
    {
        _indentation++;

        return this;
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    private SourceWriter UnIndent()
    {
        _indentation = Math.Max(0, _indentation - 1);

        return this;
    }

    /// <summary>
    /// Appends a string to the source code.
    /// </summary>
    /// <returns>The current instance of the <see cref="SourceWriter"/>.</returns>
    private SourceWriter AppendLine()
    {
        _sb.AppendLine();

        return this;
    }

    /// <summary>
    /// Appends a string (without a new line) to the source code.
    /// </summary>
    /// <param name="value">The string to append.</param>
    private void Append(char value)
    {
        _sb.Append(value);
    }
}