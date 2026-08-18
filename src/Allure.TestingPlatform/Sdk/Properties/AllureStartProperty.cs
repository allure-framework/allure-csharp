using System;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the start time of an Allure test, step, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to update.</typeparam>
/// <param name="start">The start time as Unix time in milliseconds.</param>
public sealed class AllureStartProperty<TModel>(long start) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the start time as Unix time in milliseconds.
    /// </summary>
    public long Start { get; } = start;

    /// <summary>
    /// Creates a start property from a timestamp.
    /// </summary>
    /// <param name="start">The start timestamp.</param>
    public AllureStartProperty(DateTimeOffset start) : this(start.ToUnixTimeMilliseconds())
    {
    }

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        target.Start = this.Start;
    }
}
