using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Sdk.Registration;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Provides an Allure runtime context whose current execution state is stored
/// in an <see cref="AsyncLocal{T}"/> field. This allows concurrently
/// executing tests and steps to maintain independent execution states, provided
/// that the <see cref="ExecutionContext"/> is propagated correctly.
/// </summary>
public sealed class AsyncLocalExecutionContext : IAllureExecutionContext
{
    readonly IReadOnlyLateBoundReference<IAllureRuntime> reference;

    readonly AsyncLocal<AllureExecutionState> currentState;

    /// <inheritdoc/>
    public IAllureRuntime Runtime => this.reference.Value;

    /// <inheritdoc/>
    public AllureExecutionState CurrentState => currentState.Value;

    public AsyncLocalExecutionContext(IReadOnlyLateBoundReference<IAllureRuntime> reference)
    {
        this.reference = reference;
        this.currentState = new()
        {
            Value = new(),
        };
    }

    /// <inheritdoc/>
    public TResult GetWithState<TResult>(AllureExecutionState state, Func<IAllureRuntime, TResult> function)
    {
        var originalState = currentState.Value;
        currentState.Value = state;

        try
        {
            return function(this.Runtime);
        }
        finally
        {
            currentState.Value = originalState;
        }
    }

    /// <inheritdoc/>
    public async Task<TResult> GetWithStateAsync<TResult>(AllureExecutionState state, Func<IAllureRuntime, Task<TResult>> asyncFunction)
    {
        var originalState = currentState.Value;
        currentState.Value = state;

        try
        {
            return await asyncFunction(this.Runtime);
        }
        finally
        {
            currentState.Value = originalState;
        }
    }

    /// <inheritdoc/>
    public AllureExecutionState RunWithState(AllureExecutionState state, Action<IAllureRuntime> action)
    {
        var originalState = currentState.Value;
        currentState.Value = state;

        try
        {
            action(this.Runtime);
            return currentState.Value;
        }
        finally
        {
            currentState.Value = originalState;
        }
    }

    /// <inheritdoc/>
    public async Task RunWithStateAsync(AllureExecutionState state, Func<IAllureRuntime, Task> asyncAction)
    {
        var originalState = currentState.Value;
        currentState.Value = state;

        try
        {
            await asyncAction(this.Runtime);
        }
        finally
        {
            currentState.Value = originalState;
        }
    }

    /// <inheritdoc/>
    public void Update(Func<AllureExecutionState, AllureExecutionState> transition)
    {
        currentState.Value = transition(currentState.Value);
    }
}
