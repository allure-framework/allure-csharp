using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStartProperty<TModel>(long start) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    public long Start { get; } = start;

    public AllureStartProperty(DateTimeOffset start) : this(start.ToUnixTimeMilliseconds())
    {
    }

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.start = this.Start;
    }
}