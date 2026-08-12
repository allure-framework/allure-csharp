using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

sealed class SyncStepOperationContext(IAllureRuntimeBase runtime, int level) :
    StepOperationContext(runtime, level),
    IAllureInProcessSyncStepContext
{
    public void AddParameter(Parameter parameter)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateStepResult(
            this.Level,
            (stepResult) => stepResult.Parameters.Add(parameter)
        );
    }

    public void SetName(string newName)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateStepResult(
            this.Level,
            (stepResult) => stepResult.Name = newName
        );
    }
}
