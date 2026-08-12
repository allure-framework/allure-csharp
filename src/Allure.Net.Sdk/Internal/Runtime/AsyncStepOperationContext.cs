using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

sealed class AsyncStepOperationContext(IAllureRuntimeBase runtime, int level) :
    StepOperationContext(runtime, level),
    IAllureInProcessAsyncStepContext
{
    public Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateStepResult(
            this.Level,
            (stepResult) => stepResult.Parameters.Add(parameter)
        );
        return Task.CompletedTask;
    }

    public Task SetNameAsync(string newName, CancellationToken _)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateStepResult(
            this.Level,
            (stepResult) => stepResult.Name = newName
        );
        return Task.CompletedTask;
    }
}
