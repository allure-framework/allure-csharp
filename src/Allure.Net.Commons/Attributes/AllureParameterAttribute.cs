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
    public bool Ignore { get; init; }

    /// <summary>
    /// A display name of the parameter. If unset, the source name of the parameter will be used.
    /// </summary>
    public string? Name { get; init; }
}