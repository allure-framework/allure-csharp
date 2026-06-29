using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureExceptionProperty<TObject>(Exception exception) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public Exception Value { get; } = exception;

    public void Apply(LiveAllureTestingPlatformRuntime allure, TObject obj)
    {
        obj.status = ModelFunctions.ResolveErrorStatus(allure.Configuration.FailExceptions, this.Value);
        obj.statusDetails = ModelFunctions.ToStatusDetails(this.Value);
    }
}