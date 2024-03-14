using System;
using System.Text;

namespace SaucyRegistrations.Generators.Infrastructure;

internal sealed class SourceWriter
{
    private readonly StringBuilder _sb = new();

    private int _indentation;

    internal SourceWriter Indent()
    {
        _indentation++;

        return this;
    }

    internal SourceWriter UnIndent()
    {
        _indentation = Math.Max(0, _indentation - 1);

        return this;
    }

    internal SourceWriter AppendLine()
    {
        _sb.AppendLine();

        return this;
    }

    internal SourceWriter AppendLine(char value)
    {
        _sb.Append(new string(' ', _indentation * 4)).Append(value).AppendLine();

        return this;
    }

    internal SourceWriter AppendLine(string value)
    {
        _sb.Append(new string(' ', _indentation * 4)).AppendLine(value);

        return this;
    }

    public override string ToString()
    {
        return _sb.ToString();
    }
}