using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStartProperty<TObject>(long start) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public long Start { get; } = start;

    public AllureStartProperty(DateTimeOffset start) : this(start.ToUnixTimeMilliseconds())
    {
    }

    public void Apply(ReadyAllureTestingPlatformRuntime _, TObject obj)
    {
        obj.start = this.Start;
    }
}