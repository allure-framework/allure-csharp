using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public enum DurationBase
{
    Start,
    Stop,
}

public sealed class AllureDurationProperty<TObject>(long duration) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public long Duration { get; } = duration;

    public AllureDurationProperty(TimeSpan duration) : this((long)Math.Round(duration.TotalMilliseconds))
    {
    }

    public DurationBase RelativeTo { get; init; } = DurationBase.Start;

    public void Apply(ReadyAllureTestingPlatformRuntime _, TObject obj)
    {
        if (this.RelativeTo == DurationBase.Start)
        {
            obj.stop = obj.start + this.Duration;
        }
        else
        {
            obj.start = obj.stop - this.Duration;
        }
    }
}