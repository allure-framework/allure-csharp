using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

sealed class SyncFixtureOperationContext(IAllureRuntimeBase runtime) :
    FixtureOperationContext(runtime),
    IAllureInProcessSyncFixtureContext
{
    public void AddParameter(Parameter parameter)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Parameters.Add(parameter)
        );
    }

    public void SetName(string newName)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Name = newName
        );
    }
}
