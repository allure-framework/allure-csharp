using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

sealed class AsyncFixtureOperationContext(IAllureRuntimeBase runtime) :
    FixtureOperationContext(runtime),
    IAllureInProcessAsyncFixtureContext
{
    public Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Parameters.Add(parameter)
        );
        return Task.CompletedTask;
    }

    public Task SetNameAsync(string newName, CancellationToken _)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Name = newName
        );
        return Task.CompletedTask;
    }
}
