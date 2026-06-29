using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureExceptionProperty<TModel>(Exception exception) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    public Exception Exception { get; } = exception;

    public void Apply(LiveAllureTestingPlatformRuntime allure, TModel target)
    {
        target.status = ModelFunctions.ResolveErrorStatus(
            allure.Configuration.FailExceptions,
            this.Exception
        );
        target.statusDetails = ModelFunctions.ToStatusDetails(this.Exception);
    }
}