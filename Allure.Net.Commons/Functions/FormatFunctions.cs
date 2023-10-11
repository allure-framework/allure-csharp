using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable

namespace Allure.Net.Commons.Functions;

public static class FormatFunctions
{
    public static string Format(object? value)
    {
        return Format(value, new Dictionary<Type, ITypeFormatter>());
    }

    public static string Format(
        object? value,
        IReadOnlyDictionary<Type, ITypeFormatter> formatters
    )
    {
        if (value is not null && formatters.TryGetValue(value.GetType(), out var formatter))
        {
            return formatter.Format(value);
        }

        return JsonConvert.SerializeObject(value);
    }
}
