using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureExceptionProperty<TObject>(Exception exception) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public Exception Value { get; } = exception;

    public void Apply(IAllureInfrastructure allure, TObject obj)
    {
        obj.status = ModelFunctions.ResolveErrorStatus(allure.Config.FailExceptions, this.Value);
        obj.statusDetails = ModelFunctions.ToStatusDetails(this.Value);
    }
}