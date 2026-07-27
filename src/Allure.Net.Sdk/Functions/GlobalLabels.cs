using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Allure.Model;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Functions;

/// <summary>
/// Defines a set of functions to retreive labels that affect all test results
/// of the current execution.
/// </summary>
public static class GlobalLabels
{
    /// <summary>
    /// Returns a sequence of labels defined by the <c>globalLabels</c>
    /// configuration property.
    /// </summary>
    public static IEnumerable<Label> FromConfiguration(AllureConfiguration config) =>
        from kv in config.GlobalLabels ?? []
        where !string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value)
        select new Label { Name = kv.Key, Value = kv.Value };

    /// <summary>
    /// Returns a sequence of labels defined by the variables that match the
    /// <c>ALLURE_LABEL_&lt;name>=&lt;value></c> pattern.
    /// </summary>
    /// <remarks>
    /// Use <see cref="System.Environment.GetEnvironmentVariables()"/> to get the
    /// process environment variables for this function.
    /// </remarks>
    public static IEnumerable<Label> FromEnvironmentVariables(
        IDictionary environmentVariables
    )
    {
        foreach (DictionaryEntry entry in environmentVariables)
        {
            var key = entry.Key as string;
            var value = entry.Value as string;
            if (IsLabelVariable(key, value))
            {
                var name = key.Substring(ENV_LABEL_PATTERN.Length);
                yield return new() { Name = name, Value = value };
            }
        }
    }

    static bool IsLabelVariable(
        [NotNullWhen(true)] string? name,
        [NotNullWhen(true)] string? value
    ) =>
        name is not null
            && name.Length > ENV_LABEL_PATTERN.Length
            && name.StartsWith(ENV_LABEL_PATTERN)
            && !string.IsNullOrEmpty(value);

    const string ENV_LABEL_PATTERN = "ALLURE_LABEL_";
}
