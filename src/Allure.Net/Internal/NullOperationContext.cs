using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Internal;

class NullOperationContext :
    IAllureStepContext,
    IAllureAsyncStepContext,
    IAllureFixtureContext,
    IAllureAsyncFixtureContext,
    IAllureInProcessStepContext,
    IAllureAsyncInProcessStepContext,
    IAllureInProcessFixtureContext,
    IAllureAsyncInProcessFixtureContext
{
    public void AddParameter(Parameter parameter)
    {
    }

    public void SetName(string newName)
    {
    }

    public Task SetNameAsync(string newName, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task AddParameterAsync(Parameter parameter, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public void UpdateStepResult(Action<StepResult> update)
    {
    }

    public bool TryReadStepResult<TResult>(Func<StepResult, TResult> read, [MaybeNullWhen(false)] out TResult value)
    {
        value = default;
        return false;
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
    {
    }

    public bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, [MaybeNullWhen(false)] out TResult value)
    {
        value = default;
        return false;
    }

    public static NullOperationContext Instance { get; } = new();

    public IAllureParameterSerializer ParameterSerializer { get; } = new ToStringParameterSerializer();
}