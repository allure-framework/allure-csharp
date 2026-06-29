using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDurationProperty<TModel>(long duration) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    public long Duration { get; } = duration;

    public AllureDurationProperty(TimeSpan duration) :
        this((long)Math.Round(duration.TotalMilliseconds))
    {
    }

    public AllureDurationAnchor RelativeTo { get; init; } = AllureDurationAnchor.Start;

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        if (this.RelativeTo == AllureDurationAnchor.Start)
        {
            target.stop = target.start + this.Duration;
        }
        else
        {
            target.start = target.stop - this.Duration;
        }
    }
}