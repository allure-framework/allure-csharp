using System;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the stop time of an Allure test, step, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to update.</typeparam>
/// <param name="stop">The stop time as Unix time in milliseconds.</param>
public sealed class AllureStopProperty<TModel>(long stop) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the stop time as Unix time in milliseconds.
    /// </summary>
    public long Stop { get; } = stop;

    /// <summary>
    /// Creates a stop property from a timestamp.
    /// </summary>
    /// <param name="stop">The stop timestamp.</param>
    public AllureStopProperty(DateTimeOffset stop) : this(stop.ToUnixTimeMilliseconds())
    {
    }

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        target.Stop = this.Stop;
    }
}
