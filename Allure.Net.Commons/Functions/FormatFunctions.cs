using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable

namespace Allure.Net.Commons.Functions;

/// <summary>
/// A set of functions to help with value-to-string conversion of test and
/// step arguments.
/// </summary>
public static class FormatFunctions
{
    /// <summary>
    /// Formats a given value into a string. This is a shorthand for
    /// <see cref="Format(object?, IReadOnlyDictionary{Type, ITypeFormatter})"/>
    /// with empty formatters dictionary.
    /// </summary>
    public static string Format(object? value)
    {
        return Format(value, new Dictionary<Type, ITypeFormatter>());
    }

    /// <summary>
    /// Formats a given value into a string. If the type of the value matches
    /// a formater in the formatters dictionary, the formatter is used to
    /// produce the result.
    /// 
    /// Otherwise, the value is formatted as a JSON string or undefined
    /// if serialization failed.
    /// 
    /// The serializer skips fields that contain loop references
    /// and fields that could not be serialized
    /// </summary>
    public static string Format(
        object? value,
        IReadOnlyDictionary<Type, ITypeFormatter> formatters
    )
    {
        if (value is not null && formatters.TryGetValue(value.GetType(), out var formatter))
        {
            return formatter.Format(value);
        }

        try
        {
            return JsonConvert.SerializeObject(
                value,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Error = (_, args) =>
                    {
                        args.ErrorContext.Handled = true;
                    }
                });
        }
        catch
        {
            return JsonConvert.Undefined;
        }
    }
}
