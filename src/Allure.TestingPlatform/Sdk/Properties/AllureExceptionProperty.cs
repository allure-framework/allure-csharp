using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureExceptionProperty<TObject>(Exception exception) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public Exception Value { get; } = exception;

    public void Apply(IAllureRuntime allure, TObject obj)
    {
        obj.status = ModelFunctions.ResolveErrorStatus(allure.Config.FailExceptions, this.Value);
        obj.statusDetails = ModelFunctions.ToStatusDetails(this.Value);
    }
}