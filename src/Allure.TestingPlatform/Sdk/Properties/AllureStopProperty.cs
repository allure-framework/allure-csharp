using System;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStopProperty<TObject>(long stop) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public long Stop { get; } = stop;

    public AllureStopProperty(DateTimeOffset stop) : this(stop.ToUnixTimeMilliseconds())
    {
    }

    public void Apply(ReadyAllureTestingPlatform _, TObject obj)
    {
        obj.stop = this.Stop;
    }
}