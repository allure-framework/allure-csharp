using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

class RuntimeBoundSyncFixtureContext(IAllureRuntime runtime) :
    IAllureInProcessSyncFixtureContext
{
    AllureExecutionState CurrentState => runtime.ContextApi.CurrentState;

    IAllureParameterSerializer IAllureOperationContext.ParameterSerializer =>
        runtime.ParameterSerializer;

    public void AddParameter(Parameter parameter)
    {
        runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Parameters.Add(parameter)
        );
    }

    public void SetName(string newName)
    {
        runtime.ModelApi.UpdateFixtureResult(
            (fixtureResult) => fixtureResult.Name = newName
        );
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
