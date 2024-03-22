using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Allure.ReqnrollPlugin.Configuration;

class WholeLineRegexConverter : RegexConverter
{
    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer
    ) =>
        reader.TokenType switch
        {
            JsonToken.String
                when reader.Value is string v && v.Any() && v[0] is not '/' =>
                    ReadFromString(v),
            _ => base.ReadJson(reader, objectType, existingValue, serializer)
        };

    static Regex ReadFromString(string value) =>
        new(
            value[0] is '^' ? value : $"^(?:{value})$",
            RegexOptions.IgnoreCase
                | RegexOptions.Compiled
                | RegexOptions.Singleline
        );
}
