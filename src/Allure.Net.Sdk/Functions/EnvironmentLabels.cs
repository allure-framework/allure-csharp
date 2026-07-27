using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Sdk.Functions;

public static class EnvironmentLabels
{
    /// <summary>
    /// Returns a sequence of labels defined by the environment variables in form
    /// of <c>ALLURE_LABEL_&lt;name>=&lt;value></c>
    /// </summary>
    public static IEnumerable<Label> Enumerate()
    {
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var key = entry.Key as string;
            var value = entry.Value as string;
            if (ShouldAddEnvVarAsLabel(key, value))
            {
                var name = key.Substring(ENV_LABEL_PATTERN.Length);
                yield return new() { Name = name, Value = value };
            }
        }
    }

    static bool ShouldAddEnvVarAsLabel(
        [NotNullWhen(true)] string? name,
        [NotNullWhen(true)] string? value
    ) =>
        name is not null
            && name.Length > ENV_LABEL_PATTERN.Length
            && name.StartsWith(ENV_LABEL_PATTERN)
            && !string.IsNullOrEmpty(value);

    const string ENV_LABEL_PATTERN = "ALLURE_LABEL_";
}
