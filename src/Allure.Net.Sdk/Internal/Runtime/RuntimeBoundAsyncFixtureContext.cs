using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

class RuntimeBoundAsyncFixtureContext(IAllureRuntime runtime) :
    IAllureInProcessAsyncFixtureContext
{
    AllureExecutionState CurrentState => runtime.ContextApi.CurrentState;

    IAllureParameterSerializer IAllureOperationContext.ParameterSerializer => runtime.ParameterSerializer;

    public Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Parameters.Add(parameter)
        );
        return Task.CompletedTask;
    }

    public Task SetNameAsync(string newName, CancellationToken _)
    {
        runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Name = newName
        );
        return Task.CompletedTask;
    }

    public bool TryReadFixtureResult<T>(
        Func<FixtureResult, T> read,
        [MaybeNullWhen(false)] out T result
    )
    {
        if (this.CurrentState.HasFixture)
        {
            result = runtime.ModelApi.ReadFixtureResult(read);
            return true;
        }

        result = default;
        return false;
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
    {
        if (this.CurrentState.HasFixture)
        {
            runtime.ModelApi.UpdateFixtureResult(update);
        }
    }
}
