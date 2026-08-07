using System;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
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
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allure, TModel target)
    {
        target.Status = ErrorStatus.Resolve(
            allure.Configuration.FailExceptions,
            this.Exception
        );
        target.StatusDetails = StatusDetails.FromException(this.Exception);
    }
}
