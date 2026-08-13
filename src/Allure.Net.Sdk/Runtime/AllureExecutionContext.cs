using System;
using System.Threading.Tasks;
using Allure.Sdk.Registration;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Provides a base implementation of an Allure execution context.
/// </summary>
/// <remarks>
/// Derived classes provide the storage for the current execution state.
/// </remarks>
/// <param name="runtimeReference">
/// A reference to the Allure runtime associated with this context.
/// </param>
public abstract class AllureExecutionContext(
    IReadOnlyLateBoundReference<IAllureRuntimeBase> runtimeReference
) :
    IAllureExecutionContext
{
    /// <inheritdoc/>
    public IAllureRuntimeBase Runtime => runtimeReference.Value;

    /// <inheritdoc/>
    public abstract AllureExecutionState CurrentState { get; protected set; }

    /// <inheritdoc/>
    public T GetWithState<T>(AllureExecutionState state, Func<IAllureRuntimeBase, T> function)
    {
        using ExecutionStateScope _ = new(this, state);
        return function(this.Runtime);
    }

    /// <inheritdoc/>
    public async Task<T> GetWithStateAsync<T>(
        AllureExecutionState state,
        Func<IAllureRuntimeBase, Task<T>> asyncFunction
    )
    {
        using ExecutionStateScope _ = new(this, state);
        return await asyncFunction(this.Runtime);
    }

    /// <inheritdoc/>
    public AllureExecutionState RunWithState(
        AllureExecutionState state,
        Action<IAllureRuntimeBase> action
    )
    {
        using ExecutionStateScope _ = new(this, state);
        action(this.Runtime);
        return this.CurrentState;
    }

    /// <inheritdoc/>
    public async Task RunWithStateAsync(
        AllureExecutionState state,
        Func<IAllureRuntimeBase, Task> asyncAction
    )
    {
        using ExecutionStateScope _ = new(this, state);
        await asyncAction(this.Runtime);
    }

    /// <inheritdoc/>
    public void Update(Func<AllureExecutionState, AllureExecutionState> transition)
    {
        this.CurrentState = transition(this.CurrentState);
    }

    private sealed class ExecutionStateScope : IDisposable
    {
        readonly AllureExecutionContext context;
        readonly AllureExecutionState originalState;

        public ExecutionStateScope(AllureExecutionContext context, AllureExecutionState state)
        {
            this.context = context;
            this.originalState = context.CurrentState;
            context.CurrentState = state;
        }

        public void Dispose()
        {
            this.context.CurrentState = this.originalState;
        }
    }
}
