using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Controls how Allure treats test and step parameters created from method arguments.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class AllureParameterAttribute : Attribute
{
    /// <summary>
    /// If set to true, the argument will be precluded from the report.
    /// </summary>
    public bool Ignore { get; init; } = false;

    /// <summary>
    /// A display name of the parameter. If unset, the source name of the parameter will be used.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// A display mode of the parameter.
    /// </summary>
    /// <remarks>
    /// This property controls how Allure displays the value of the parameter. It doesn't affect
    /// how the value is represented in result files.
    /// </remarks>
    public ParameterMode Mode { get; init; } = ParameterMode.Default;

    /// <summary>
    /// If set to <c>true</c>, the parameter doesn't affect the test's historyId.
    /// Use for timestamps, random values, and other values that may change across runs by design.
    /// </summary>
    /// <remarks>
    /// Setting this property doesn't remove the parameter from the report. To remove the parameter
    /// entirely, use <see cref="Ignore"/>.
    /// <br></br>
    /// Has no effect when applied to a step's parameter.
    /// </remarks>
    public bool Excluded { get; init; } = false;
}