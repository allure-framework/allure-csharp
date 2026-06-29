using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStopProperty<TModel>(long stop) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    public long Stop { get; } = stop;

    public AllureStopProperty(DateTimeOffset stop) : this(stop.ToUnixTimeMilliseconds())
    {
    }

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.stop = this.Stop;
    }
}