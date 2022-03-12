using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MGen.Abstractions.Builders.Components;

public partial class XmlCommentsBuilder
{
    [DebuggerStepThrough]
    internal readonly ref struct ElementInfo
    {
        public ElementInfo()
        {
            After = ReadOnlySpan<char>.Empty;
            Attributes = ReadOnlySpan<char>.Empty;
            Before = ReadOnlySpan<char>.Empty;
            Closure = ElementClosure.None;
            Element = ReadOnlySpan<char>.Empty;
            Name = ReadOnlySpan<char>.Empty;
        }

        public ElementInfo(ReadOnlySpan<char> before, ReadOnlySpan<char> element, ReadOnlySpan<char> after)
        {
            if (element.Length < 3 ||
                element[0] != '<' ||
                element[element.Length - 1] != '>')
            {
                throw new InvalidEnumArgumentException("Invalid element value: " + element.ToString());
            }

            Before = before;
            Element = element;
            After = after;

            GetElementDetails(element,
                out var closure,
                out var name,
                out var attributes);

            Attributes = attributes;
            Closure = closure;
            Name = name;
        }

        public ElementClosure Closure { get; }

        public ReadOnlySpan<char> Attributes { get; }

        public ReadOnlySpan<char> Before { get; }

        public ReadOnlySpan<char> Element { get; }

        public ReadOnlySpan<char> Name { get; }

        public ReadOnlySpan<char> After { get; }

        public static ElementInfo Empty => new();

        public static bool Next(ReadOnlySpan<char> buffer, out ElementInfo elementInfo)
        {
            var start = buffer.IndexOf('<');
            if (start == -1)
            {
                elementInfo = Empty;
                return false;
            }

            var element = buffer.Slice(start);

            var stop = element.IndexOf('>');
            if (stop == -1)
            {
                elementInfo = Empty;
                return false;
            }

            var before = buffer.Slice(0, start);

            elementInfo = new(before, element.Slice(0, stop + 1), element.Slice(stop + 1));

            return true;
        }

        public string? GetAttribute(ReadOnlySpan<char> name)
        {
            int GetLengthOfNOnQuotedAttributeValue(ReadOnlySpan<char> buffer)
            {
                for (var index = 0; index < buffer.Length; index++)
                {
                    var c = buffer[index];
                    if (char.IsWhiteSpace(c) || c is '>' or '/')
                    {
                        return index;
                    }
                }

                return buffer.Length;
            }

            var attributes = Attributes;

            var equalIndex = Attributes.IndexOf('=');
            while (equalIndex != -1)
            {
                var attributeName = attributes.Slice(0, equalIndex).TrimEnd();

                attributes = equalIndex + 1 == attributes.Length ? ReadOnlySpan<char>.Empty : attributes.Slice(equalIndex + 1);

                if (attributes.IsEmpty)
                {
                    return attributeName.Equals(name, StringComparison.Ordinal) ? string.Empty : null;
                }

                var attributesTrimmed = attributes.TrimStart();
                var quote = attributesTrimmed[0];
                if (quote is '\"' or '\'')
                {
                    attributes = attributesTrimmed.Slice(1);

                    var lastQuoteIndex = attributes.IndexOf(quote);

                    if (lastQuoteIndex == -1)
                    {
                        lastQuoteIndex = GetLengthOfNOnQuotedAttributeValue(attributes);
                    }

                    if (attributeName.Equals(name, StringComparison.Ordinal))
                    {
                        return attributes.Slice(0, lastQuoteIndex).ToString();
                    }

                    attributes = attributes.Slice(lastQuoteIndex + 1).TrimEnd();
                }
                else
                {
                    var valueLength = GetLengthOfNOnQuotedAttributeValue(attributes);

                    if (attributeName.Equals(name, StringComparison.Ordinal))
                    {
                        return attributes.Slice(0, valueLength).ToString();
                    }

                    if (valueLength == attributes.Length)
                    {
                        return null;
                    }

                    attributes = valueLength < attributes.Length ? attributes.Slice(valueLength + 1).TrimEnd() : ReadOnlySpan<char>.Empty;
                }

                equalIndex = attributes.IndexOf('=');
            }

            return null;
        }

        public void CopyContentsTo(List<string> lines, out ReadOnlySpan<char> after)
        {
            after = After;

            if (Closure != ElementClosure.StartOfElement)
            {
                return;
            }

            var contentLength = 0;

            while (Next(after, out var child))
            {
                after = child.After;
                contentLength += child.Before.Length;

                if (child.Name.Equals(Name, StringComparison.Ordinal) &&
                    child.Closure == ElementClosure.EndOfElement)
                {
                    break;
                }

                contentLength += child.Element.Length;
            }

            var contents = contentLength > 0 ? After.Slice(0, contentLength) : After;

            var index = contents.IndexOf('\n');
            while (index != -1)
            {
                var line = contents.Slice(0, index);

                contents = index + 1 == contents.Length ? ReadOnlySpan<char>.Empty : contents.Slice(index + 1);

                if (lines.Count > 0 || !line.IsEmpty && (line.Length > 1 || line[0] != '\r'))
                {
                    lines.Add(line.TrimStart().Trim('\r').ToString());
                }

                index = contents.IndexOf('\n');
            }

            if (!contents.IsEmpty && !contents.IsWhiteSpace())
            {
                lines.Add(contents.TrimStart().Trim('\r').ToString());
            }
        }

        static void GetElementDetails(ReadOnlySpan<char> element,
            out ElementClosure closure,
            out ReadOnlySpan<char> name,
            out ReadOnlySpan<char> attributes)
        {
            var buffer = element.Slice(1, element.Length - 2).Trim();
            if (buffer[0] == '/')
            {
                closure = ElementClosure.EndOfElement;
                buffer = buffer.Slice(1).TrimStart();
            }
            else if (buffer[buffer.Length - 1] == '/')
            {
                closure = ElementClosure.StartAndEndOfElement;
                buffer = buffer.Slice(0, buffer.Length - 1).TrimEnd();
            }
            else
            {
                closure = ElementClosure.StartOfElement;
            }

            var nameLength = buffer.Length;
            for (var index = 0; index < buffer.Length; index++)
            {
                if (!char.IsLetter(buffer[index]) &&
                    !char.IsDigit(buffer[index]) &&
                    buffer[index] != '-' &&
                    buffer[index] != '_' &&
                    buffer[index] != '.' &&
                    buffer[index] != ':')
                {
                    nameLength = index;
                    break;
                }
            }

            attributes = closure == ElementClosure.EndOfElement ? ReadOnlySpan<char>.Empty : buffer.Slice(nameLength).TrimStart();
            name = buffer.Slice(0, nameLength);
        }
    }
}