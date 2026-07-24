using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal;

namespace Allure.Sdk.Functions;

/// <summary>
/// Contains functions to help implementing Allure model-related conversions.
/// </summary>
public static class ModelFunctions
{
    /// <summary>
    /// Checks if an exception type, one of its base types, or one of the
    /// interfaces it implements exists in the list of known execption types.
    /// </summary>
    /// <param name="knownErrorBases">The list of known exception types.</param>
    /// <param name="e">The exception to check.</param>
    public static bool IsKnownError(IEnumerable<string> knownErrorBases, Exception e) =>
        knownErrorBases
            ?.Intersect(
                GetExceptionClassChain(e)
            )
            ?.Any() == true;

    /// <summary>
    /// Returns a <see cref="Status.Failed"/> if a given exception represents
    /// an assertion error. Otherwise, returns <see cref="Status.Broken"/>.
    /// </summary>
    /// <param name="failExceptions">
    ///   The list of exception types. Exceptions of those types (including
    ///   subclasses) are considered assertion errors. This list typically comes
    ///   from the configuration associated with the current lifecycle instance.
    /// </param>
    /// <param name="e">The exception to convert.</param>
    /// <returns></returns>
    public static Status ResolveErrorStatus(
        IEnumerable<string> failExceptions,
        Exception e
    ) =>
        IsKnownError(failExceptions, e)
            ? Status.Failed
            : Status.Broken;

    /// <summary>
    /// Converts an exception to the status details.
    /// </summary>
    /// <param name="e">The exception to convert.</param>
    public static StatusDetails? ToStatusDetails(Exception? e) =>
        e is null
            ? null
            : new()
            {
                Message = string.IsNullOrEmpty(e.Message)
                    ? e.GetType().Name
                    : e.Message,
                Trace = e.ToString()
            };

    /// <summary>
    /// Checks if the test result contains a suite-hierarchy label, i.e., one
    /// of the <c>parentSuite</c>, <c>suite</c>, or <c>subSuite</c> labels. If
    /// not, adds the provided default values to the list of labels. Otherwise,
    /// leaves the test result as is.
    /// </summary>
    /// <param name="testResult">A test result to modify</param>
    /// <param name="parentSuite">
    /// A value for the <c>parentSuite</c> label. If null or empty, the label
    /// won't be added
    /// </param>
    /// <param name="suite">
    /// A value for the <c>suite</c> label. If null or empty, the label won't
    /// be added
    /// </param>
    /// <param name="subSuite">
    /// A value for the <c>subSuite</c> label. If null or empty, the label won't
    /// be added
    /// </param>
    public static void EnsureSuites(
        TestResult testResult,
        string? parentSuite,
        string? suite,
        string? subSuite
    )
    {
        var labels = testResult.Labels;
        if (labels.Any(IsSuiteLabel))
        {
            return;
        }

        if (!string.IsNullOrEmpty(parentSuite))
        {
            labels.Add(Label.ParentSuite(parentSuite!));
        }

        if (!string.IsNullOrEmpty(suite))
        {
            labels.Add(Label.Suite(suite!));
        }

        if (!string.IsNullOrEmpty(subSuite))
        {
            labels.Add(Label.SubSuite(subSuite!));
        }
    }

    /// <summary>
    /// Returns a sequence of labels defined by the environment variables in form
    /// of <c>ALLURE_LABEL_&lt;name>=&lt;value></c>
    /// </summary>
    public static IEnumerable<Label> EnumerateEnvironmentLabels()
    {
        foreach (DictionaryEntry entry in GetEnvironmentVariables())
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

    /// <summary>
    /// Returns a sequence of labels defined by the <c>globalLabels</c>
    /// configuration property.
    /// </summary>
    public static IEnumerable<Label> EnumerateGlobalLabels(AllureConfiguration config) =>
        from kv in config.GlobalLabels ?? []
        where !string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value)
        select new Label { Name = kv.Key, Value = kv.Value };

    public static IEnumerable<Parameter> CreateParameters(
        IEnumerable<ParameterInfo> parameters,
        IEnumerable<object?> values,
        IAllureParameterSerializer parameterSerializer
    )
        => CreateParameters(
            parameters.Select(static (p) => p.Name),
            parameters.Select(static (p) => p.GetCustomAttribute<AllureParameterAttribute>()),
            values,
            parameterSerializer
        );

    public static IEnumerable<Parameter> CreateParameters(
        IEnumerable<string> parameterNames,
        IEnumerable<AllureParameterAttribute?> attributes,
        IEnumerable<object?> values,
        IAllureParameterSerializer parameterSerializer
    )
        => parameterNames
            .Zip(attributes, static (n, a) => (name: n, attr: a))
            .Zip(values, static (p, v) => (p.name, p.attr, value: v))
            .Where(static (tuple) => tuple.attr?.Ignore is not true)
            .Select((tuple) =>
                CreateParameter(tuple.name, tuple.attr, tuple.value, parameterSerializer));

    /// <summary>
    /// Returns a name for an attachment file.
    /// </summary>
    /// <param name="fileExtension">An optional file extension.</param>
    public static string GetAttachmentSourceName(string fileExtension = "")
    {
        fileExtension ??= "";
        var suffix = "-attachment";
        var uuid = IdFunctions.CreateUUID();
        return $"{uuid}{suffix}{fileExtension}";
    }

    public static void ApplyLinkTemplates(
        IReadOnlyDictionary<string, AllureLinkTemplate> templates,
        Link link
    )
    {
        if (templates.TryGetValue(link.Type ?? "link", out var template))
        {
            ApplyLinkTemplate(template, link);
        }
    }

    static void ApplyLinkTemplate(AllureLinkTemplate template, Link link)
    {
        if (Uri.IsWellFormedUriString(link.Url, UriKind.Absolute))
        {
            return;
        }

        var (urlTemplate, nameTemplate) = template;

        var urlInput = link.Url;
        link.Url = string.Format(template.UrlTemplate, urlInput);

        if (nameTemplate is null)
        {
            return;
        }

        var nameInput = link.Name ?? urlInput;
        link.Name = string.Format(nameTemplate, nameInput);
    }

    static Parameter CreateParameter(
        string parameterName,
        AllureParameterAttribute? attribute,
        object? value,
        IAllureParameterSerializer parameterSerializer
    )
        => new()
        {
            Name = attribute?.Name ?? parameterName,
            Value = parameterSerializer.Serialize(value),
            Excluded = attribute?.Excluded == true,
            Mode = ResolveParameterMode(attribute)
        };

    static ParameterMode? ResolveParameterMode(AllureParameterAttribute? attribute)
        => attribute is AllureParameterAttribute { Mode: ParameterMode mode and not ParameterMode.Default }
            ? mode
            : null;

    static bool ShouldAddEnvVarAsLabel(
        [NotNullWhen(true)] string? name,
        [NotNullWhen(true)] string? value
    ) =>
        name is not null
            && name.Length > ENV_LABEL_PATTERN.Length
            && name.StartsWith(ENV_LABEL_PATTERN)
            && !string.IsNullOrEmpty(value);

    const string ENV_LABEL_PATTERN = "ALLURE_LABEL_";

    static bool IsSuiteLabel(Label label) => label.Name switch
    {
        LabelName.ParentSuite or LabelName.Suite or LabelName.SubSuite => true,
        _ => false
    };

    static IEnumerable<string> GetExceptionClassChain(Exception e) =>
        TypeFunctions.GetTypeClosure(e.GetType())
            .Select(static (t) => t.FullName);

    #region For testing

    static IDictionary GetEnvironmentVariables() =>
        (GetEnvironmentVariablesBox.Value
            ?? Environment.GetEnvironmentVariables).Invoke();

    internal static void SetGetEnvironmentVariables(Func<IDictionary>? getEnvVars) =>
        GetEnvironmentVariablesBox.Value = getEnvVars;

    // To decouple from Environment.GetEnvironmentVariable
    static AsyncLocal<Func<IDictionary>?> GetEnvironmentVariablesBox { get; set; } = new();

    // To decouple from AllureLifecycle.Instance.AllureConfiguration
    static AsyncLocal<AllureConfiguration?> ConfigBox { get; set; } = new();

    #endregion
}
