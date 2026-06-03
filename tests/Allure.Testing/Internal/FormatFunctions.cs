using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Allure.Testing.Internal;

public static class FormatFunctions
{
    public static string FormatAsStringLiteral<T>(T value) =>
        value?.ToString() is { } text ? FormatStringLiteral(text) : "<null>";

    /// <summary>
    /// <see href="https://github.com/dotnet/roslyn/blob/e3a102fb75ef112d064feebd2f9385385a445a06/src/Compilers/CSharp/Portable/SymbolDisplay/ObjectDisplay.cs#L212"/>
    /// </summary>
    public static string FormatStringLiteral(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        const char quote = '"';

        var builder = new StringBuilder();

        var useQuotes = true;
        var escapeNonPrintable = true;

        var isVerbatim = useQuotes && !escapeNonPrintable && ContainsNewLine(value);

        if (useQuotes)
        {
            if (isVerbatim)
            {
                builder.Append('@');
            }
            builder.Append(quote);
        }

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (escapeNonPrintable && CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.Surrogate)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(value, i);
                if (category == UnicodeCategory.Surrogate)
                {
                    // an unpaired surrogate
                    builder.Append("\\u" + ((int)c).ToString("x4"));
                }
                else if (NeedsEscaping(category))
                {
                    // a surrogate pair that needs to be escaped
                    var unicode = char.ConvertToUtf32(value, i);
                    builder.Append("\\U" + unicode.ToString("x8"));
                    i++; // skip the already-encoded second surrogate of the pair
                }
                else
                {
                    // copy a printable surrogate pair directly
                    builder.Append(c);
                    builder.Append(value[++i]);
                }
            }
            else if (escapeNonPrintable && TryReplaceChar(c, out var replaceWith))
            {
                builder.Append(replaceWith);
            }
            else if (useQuotes && c == quote)
            {
                if (isVerbatim)
                {
                    builder.Append(quote);
                    builder.Append(quote);
                }
                else
                {
                    builder.Append('\\');
                    builder.Append(quote);
                }
            }
            else
            {
                builder.Append(c);
            }
        }

        if (useQuotes)
        {
            builder.Append(quote);
        }

        return builder.ToString();
    }

    static bool ContainsNewLine(string s)
    {
        foreach (char c in s)
        {
            if (IsNewLine(c))
            {
                return true;
            }
        }

        return false;
    }

    static bool NeedsEscaping(UnicodeCategory category) => category switch
    {
        UnicodeCategory.Control
            or UnicodeCategory.OtherNotAssigned
            or UnicodeCategory.ParagraphSeparator
            or UnicodeCategory.LineSeparator
            or UnicodeCategory.Surrogate =>
                true,

        _ => false,
    };

    /// <summary>
    /// <see cref="https://github.com/dotnet/roslyn/blob/e3a102fb75ef112d064feebd2f9385385a445a06/src/Compilers/CSharp/Portable/Parser/CharacterInfo.cs#L149"/>
    /// </summary>
    static bool IsNewLine(char ch) =>
        ch is '\r' or '\n' or '\u0085' or '\u2028' or '\u2029';

    static bool TryReplaceChar(char c, [NotNullWhen(true)] out string? replaceWith)
    {
        replaceWith = null;
        switch (c)
        {
            case '\\':
                replaceWith = "\\\\";
                break;
            case '\0':
                replaceWith = "\\0";
                break;
            case '\a':
                replaceWith = "\\a";
                break;
            case '\b':
                replaceWith = "\\b";
                break;
            case '\f':
                replaceWith = "\\f";
                break;
            case '\n':
                replaceWith = "\\n";
                break;
            case '\r':
                replaceWith = "\\r";
                break;
            case '\t':
                replaceWith = "\\t";
                break;
            case '\v':
                replaceWith = "\\v";
                break;
        }

        if (replaceWith != null)
        {
            return true;
        }

        if (NeedsEscaping(CharUnicodeInfo.GetUnicodeCategory(c)))
        {
            replaceWith = "\\u" + ((int)c).ToString("x4");
            return true;
        }

        return false;
    }
}