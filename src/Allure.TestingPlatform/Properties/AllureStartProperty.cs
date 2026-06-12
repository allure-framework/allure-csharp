using System;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureStartProperty<TObject>(long start) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public long Start { get; } = start;

    public AllureStartProperty(DateTimeOffset start) : this(start.ToUnixTimeMilliseconds())
    {
    }

    public void Apply(IAllureInfrastructure _, TObject obj)
    {
        obj.start = this.Start;
    }
}