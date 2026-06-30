using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the stop time of an Allure test, step, or fixture.
/// </summary>
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
    public AllureStopProperty(DateTimeOffset stop) : this(stop.ToUnixTimeMilliseconds())
    {
    }

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.stop = this.Stop;
    }
}
