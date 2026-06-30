using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets test, step, or fixture status and status details from an exception.
/// </summary>
public sealed class AllureExceptionProperty<TModel>(Exception exception) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the exception used to update the model.
    /// </summary>
    public Exception Exception { get; } = exception;

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime allure, TModel target)
    {
        target.status = ModelFunctions.ResolveErrorStatus(
            allure.Configuration.FailExceptions,
            this.Exception
        );
        target.statusDetails = ModelFunctions.ToStatusDetails(this.Exception);
    }
}
