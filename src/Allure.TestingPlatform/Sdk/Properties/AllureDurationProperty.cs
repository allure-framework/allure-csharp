using System;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the duration of an Allure test, step, or fixture.
/// </summary>
public sealed class AllureDurationProperty<TModel>(long duration) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the duration in milliseconds.
    /// </summary>
    public long Duration { get; } = duration;

    /// <summary>
    /// Creates a duration property from a time span.
    /// </summary>
    public AllureDurationProperty(TimeSpan duration) :
        this((long)Math.Round(duration.TotalMilliseconds))
    {
    }

    /// <summary>
    /// Gets or sets which existing timestamp remains fixed.
    /// </summary>
    public AllureDurationAnchor RelativeTo { get; init; } = AllureDurationAnchor.Start;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        if (this.RelativeTo == AllureDurationAnchor.Start)
        {
            target.Stop = target.Start + this.Duration;
        }
        else
        {
            target.Start = target.Stop - this.Duration;
        }
    }
}
